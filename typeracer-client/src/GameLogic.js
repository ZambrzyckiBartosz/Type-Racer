import { useState, useEffect, useRef, useMemo } from 'react';
import { HubConnectionBuilder } from '@microsoft/signalr';

export const useGameLogic = () => {
    const [connection, setConnection] = useState(null);
    const inputRef = useRef(null);

    const [session, setSession] = useState({ isAuth: false, username: "" });
    const [room, setRoom] = useState({ code: "", isJoined: false, players: [], chat: [], opponents: {}, host: "", settings: { powerUpsEnabled:false, hardMode: false, secondsToEnd: 0} });
    const [game, setGame] = useState({ status: 'lobby', text: "Loading...", countdown: 0, winner: "", leaderboard: [] });
    const [player, setPlayer] = useState({ input: "", progress: 0, wpm: 0, hasError: false, totalKeys: 0, wrongKeys: 0, powerUp: null, debuff: null, buff: null });

    const powerUpProgress = useMemo(() => {
        if (player.powerUp) return 100;

        let correctLength = 0;
        for (let i = 0; i < player.input.length; i++) {
            if (player.input[i] !== game.text[i]) break;
            correctLength = i + 1;
        }

        let progress = (correctLength % 5) / 5 * 100;

        if (correctLength > 0 && correctLength % 5 === 0) {
            progress = 100;
        }

        return progress;
    }, [player.input, game.text, player.powerUp]);

    useEffect(() => {
        const newConnection = new HubConnectionBuilder()
            .withUrl("http://localhost:8080/gamehub", {
            	accessTokenFactory: () => localStorage.getItem("token")
            })
            .withAutomaticReconnect()
            .build();
        setConnection(newConnection);
    }, []);

    useEffect(() => {
        const savedToken = localStorage.getItem('token');
        const savedUsername = localStorage.getItem('username');

        if (!savedToken || savedToken === "undefined" || !savedUsername || savedUsername === "undefined") {
            localStorage.removeItem('token');
            localStorage.removeItem('username');
            setSession({ isAuth: false, username: "" });
        } else {
            setSession({ isAuth: true, username: savedUsername });
        }
    }, []);

    useEffect(() => {
        if (session.isAuth && game.status === 'finished' && session.username) {
            const sendStats = async () => {
                try {
                    const token = localStorage.getItem('token');
                    const isWinner = game.winner === session.username;
                    const response = await fetch("http://localhost:8080/api/save-score", {
                        method: 'POST',
                        headers: {
                            'Content-Type': 'application/json',
                            'Authorization': `Bearer ${token}`
                        },
                        body: JSON.stringify({ Username: session.username, Wpm: player.wpm, IsWinner: isWinner })
                    });
                    if (!response.ok) console.error("Stats not saved");
                } catch (error) {
                    console.error("Error occurred while saving stats");
                }
            };
            sendStats();
        }
    }, [game.status, session.isAuth, session.username, game.winner, player.wpm]);

    useEffect(() => {
        if (session.isAuth && (game.status === 'lobby' || game.status === 'finished')) {
            const getStats = async () => {
                try {
                    const response = await fetch("http://localhost:8080/api/leaderboard", {
                        headers: { 'Content-Type': 'application/json' }
                    });
                    if (response.ok){
                         const data = await response.json();
                         setGame(g => ({ ...g, leaderboard: data})); 
                    }
                } catch (error) {
                    console.error("Leaderboard error");
                }
            };
            getStats();
        }
    }, [session.isAuth, game.status]);

    useEffect(() => {
        if (game.countdown > 0) {
            const timer = setTimeout(() => setGame(g => ({ ...g, countdown: g.countdown - 1 })), 1000);
            return () => clearTimeout(timer);
        } else if (game.countdown === 0 && game.status === 'countdown') {
            setGame(g => ({ ...g, status: 'racing' }));
            if (inputRef.current) inputRef.current.focus();
        }
    }, [game.countdown, game.status]);

    useEffect(() => {
        if (player.debuff && player.debuff !== 'freeze' && inputRef.current) {
            inputRef.current.focus();
        }    
    }, [player.debuff]);

    useEffect(() => {
        if (connection) {
            connection.start().then(() => {
                console.log("Connected with server");

                connection.on("UpdateState", (gameState) => {
                    const currentLocalPlayer = localStorage.getItem('username');
                    if (gameState.playerNick === currentLocalPlayer) {
                        setPlayer(p => ({ ...p, progress: gameState.progress, hasError: gameState.hasError, wpm: gameState.wpm }));
                    } else {
                        setRoom(r => ({ ...r, opponents: { ...r.opponents, [gameState.playerNick]: { progress: gameState.progress, wpm: gameState.wpm } } }));
                    }
                });

                connection.on("UpdatePlayersList", (data) => setRoom(r => ({ ...r, players: data.players, host: data.host })));

                connection.on("BackToLobby", () => {
                    setGame(g => ({ ...g, status: 'lobby', countdown: 0, winner: "", text: "Loading..." }));
                    setPlayer({ input: "", progress: 0, wpm: 0, hasError: false, totalKeys: 0, wrongKeys: 0, powerUp: null, debuff: null, buff: null });
                    setRoom(r => ({ ...r, opponents: {} }));
                });

                connection.on("GameOver", (winnerName) => setGame(g => ({ ...g, winner: winnerName, status: 'finished' })));
                
                connection.on("PowerUpGranted", (power) => setPlayer(p => ({ ...p, powerUp: power })));

                connection.on("SetUpLobby", (settings) => {
                	setRoom(r => ({...r , settings: {
                		powerUpsEnabled: settings.powerUpsEnabled,
                		hardMode: settings.hardMode,
                		secondsToEnd: settings.secondsToEnd
                	}}));	
                });

                connection.on("SettingsUpdate", (newSettings) => {
                	setRoom(r => ({...r, settings: {
                		powerUpsEnabled: newSettings.powerUpsEnabled,
                		hardMode: newSettings.hardMode,
                		secondsToEnd: newSettings.secondsToEnd
                	}}));	
                });

                connection.on("ReceiveAttack", (targetNick, power) => {
                    if (targetNick === localStorage.getItem('username')) {
                        setPlayer( p=> {
                             if(p.buff === 'shield'){
                                 return {...p, buff: null};
                             }    
                             if (power === 'bomb') {
                                 const cutLength = Math.max(0, p.input.length - 10);
                                 return { ...p, input: p.input.substring(0, cutLength), debuff: 'bomb' };
                         } else {
                                 return { ...p, debuff: power };
                              }
                        });

                        if(power === 'bomb'){
                            setTimeout(() => setPlayer(p => p.debuff === 'bomb' ? {...p, debuff:null} : p), 1000);
                        }
                        else{
                            setTimeout(() => setPlayer(p => p.debuff === power ? {...p, debuff:null} : p), 3000);
                        }
                    }
                });

                connection.on("ReceiveDefense", (nickname, power) => {
                    setPlayer(p => ({...p, buff: power}));
                    setTimeout(() => setPlayer(p => ({ ...p, buff: null})),    1500);    
                });

                connection.on("ReceiveChatMessage", (senderNick, messageText) => {
                    setRoom(r => ({
                        ...r, 
                        chat: [...r.chat, { text: messageText, sender: senderNick, type: senderNick === localStorage.getItem('username') ? "sent" : "received" }]
                    }));
                });
                
                connection.on("LoadText", (newText) => {
                    setGame(g => ({ ...g, text: newText, countdown: 3, status: 'countdown', winner: "" }));
                    setPlayer({ input: "", progress: 0, wpm: 0, hasError: false, totalKeys: 0, wrongKeys: 0, powerUp: null, debuff: null, buff: null });
                    setRoom(r => ({ ...r, opponents: {} }));
                });
            }).catch(e => console.log("Connection error: " + e));
        }
    }, [connection]);

    const handleInputChange = async (e) => {
        if (game.status !== 'racing' || player.debuff === 'freeze') return;
        const text = e.target.value;
        
        setPlayer(prev => {
            let newTotal = prev.totalKeys;
            let newWrong = prev.wrongKeys;
            if (text.length > prev.input.length) {
                newTotal++;
                if (text !== game.text.substring(0, text.length)) newWrong++;
            }
            return { ...prev, input: text, totalKeys: newTotal, wrongKeys: newWrong };
        });
        
        if (connection?.state === "Connected") {
            try { await connection.invoke("SendProgress", text); } 
            catch(e) { console.error("Send error: ", e); }
        }
    };

    const handleSpecialKeys = async (e) => {
        if(e.key === 'Control'){
            e.preventDefault();
            if(!player.powerUp) return;

            const enemies = room.players.filter(p => p !== session.username);
            if(enemies.length === 0) return;
            const randomEnemy = enemies[Math.floor(Math.random() * enemies.length)];    
            handleUsePowerUp(randomEnemy);        
        }    
    };

    const handleRestart = async () => { if (connection?.state === "Connected") await connection.invoke("RestartGame"); };
    
    const sendChatMessage = async (messageText) => {
        if (connection?.state === "Connected" && room.code) await connection.invoke("SendChatMessage", room.code, session.username, messageText);
    };

    const handleUsePowerUp = async (targetNick) => {
        if (connection?.state === "Connected" && player.powerUp) {
            await connection.invoke("UsePowerUp", room.code, session.username, targetNick, player.powerUp);
            setPlayer(p => ({ ...p, powerUp: null }));
        }
    };
    
    const handleJoinRooms = async () => {
        if (connection?.state === "Connected" && room.code.trim() !== "") {
            const isStarted = await connection.invoke("JoinRoom", room.code);
            if (isStarted) setRoom(r => ({ ...r, isJoined: true }));
            else alert("Game already started");
        }
    };

    const handleStart = async () => {
        if (connection?.state === "Connected" && room.code) await connection.invoke("StartRoomGame", room.code);
    };

	const handleChangeSettings = async (powerUps, hard, seconds) => {
		console.log(typeof seconds);
		if(connection?.state === "Connected" && room.code && session.username === room.host){
			await connection.invoke("ChangeRoomSettings", room.code, powerUps, hard, seconds);
		}	
	};
	
    const renderHighlightedText = () => {
        let firstErrorIndex = player.input.length;
        for (let i = 0; i < player.input.length; i++) {
            if (player.input[i] !== game.text[i]) { firstErrorIndex = i; break; }
        }
        return game.text.split("").map((char, index) => {
            let color = "#777", backgroundColor = "transparent", borderBottom = "none", opacity = "1";
            if (index < player.input.length) {
                if (index < firstErrorIndex) color = "#4caf50";
                else { color = "#ffffff"; backgroundColor = "#f44336"; }
            } else if (index === player.input.length && game.status === 'racing') {
                borderBottom = "3px solid #2196f3"; color = "#ffffff";
            } else { opacity = "0.6"; }

            return (
                <span key={index} style={{ color, backgroundColor, borderBottom, opacity, paddingBottom: '2px', borderRadius: '2px' }}>
                    {char}
                </span>
            );
        });
    };

    const accuracy = player.totalKeys > 0 ? Math.round(((player.totalKeys - player.wrongKeys) / player.totalKeys) * 100) : 67;    

    return {
        session,
        room,
        game,
        player,
        computed: {
            accuracy,
            powerUpProgress
        },
        actions: {
            setIsAuthenticated: (val) => setSession(s => ({ ...s, isAuth: val })),
            setCurrentPlayer: (val) => setSession(s => ({ ...s, username: val })),
            setRoomCode: (val) => setRoom(r => ({ ...r, code: val })),
            handleInputChange,
            handleRestart,
            handleJoinRooms,
            handleStart,
            sendChatMessage,
            handleUsePowerUp,
            renderHighlightedText,
            handleSpecialKeys,
            handleChangeSettings
        },
        inputRef
    };
};
