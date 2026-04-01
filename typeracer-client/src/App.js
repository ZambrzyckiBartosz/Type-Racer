import React, { useState, useEffect, useRef } from 'react';
import Auth from './Auth';
import { useGameLogic } from './GameLogic';
import './Chat.css';
import './Game.css';

function Chat({ currentPlayer, chatMessages, sendChatMessage }) {
    const [inputValue, setInputValue] = useState("");
    const messagesEndRef = useRef(null);

    const scrollToBottom = () => {
        messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
    };

    useEffect(() => {
        scrollToBottom();
    }, [chatMessages]);

    const handleSend = () => {
        if (inputValue.trim() === "") return;
        sendChatMessage(inputValue.trim());
        setInputValue("");
    };

    const handleKeyDown = (e) => {
        if (e.key === 'Enter') {
            handleSend();
        }
    };

    return (
        <div className="glass-panel-sm chat-container" style={{ width: '100%', maxWidth: '400px', margin: '0 auto', padding: '15px' }}>
            <div className="chat-header" style={{ marginBottom: '10px' }}>
                <h3 style={{ margin: 0, fontSize: '1.1rem', color: '#00f0ff' }}>Room Chat</h3>
            </div>
            
            <div className="message-display" style={{ maxHeight: '150px', overflowY: 'auto', marginBottom: '10px' }}>
                {chatMessages && chatMessages.map((msg, index) => (
                    <div key={index} className={`message ${msg.type}`} style={{ padding: '5px', color: msg.type === 'received' ? '#ccc' : '#fff' }}>
                        {msg.type === "received" && (
                            <div style={{ fontSize: '0.7rem', opacity: 0.7, color: '#bd00ff' }}>{msg.sender}</div>
                        )}
                        {msg.text}
                    </div>
                ))}
                <div ref={messagesEndRef} />
            </div>

            <div className="chat-input-area" style={{ display: 'flex', gap: '10px' }}>
                <input 
                    type="text" 
                    className="cyber-input"
                    value={inputValue}
                    onChange={(e) => setInputValue(e.target.value)}
                    onKeyDown={handleKeyDown}
                    placeholder="Type a message..." 
                    autoComplete="off"
                    style={{ padding: '10px', fontSize: '0.9rem' }}
                />
                <button className="btn btn-primary" onClick={handleSend} style={{ padding: '10px 15px' }}>
                    Send
                </button>
            </div>
        </div>
    );
}

function App() {
    const { 
        session, 
        room, 
        game, 
        player, 
        computed, 
        actions, 
        inputRef 
    } = useGameLogic();

    return (
        <div className={`game-container ${player.debuff === 'flashbang' ? 'flashbang-active' : ''} ${player.debuff === 'bomb' ? 'bomb-active' : ''}`}>
            
            <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '40px' }}>
                <h1 className="title-glow">TypeRacer</h1>
                {session.isAuth && (
                    <div style={{ textAlign: 'right', display: 'flex', alignItems: 'center', gap: '15px' }}>
                        <span style={{ color: '#00f0ff', fontSize: '18px' }}>{session.username}</span>
                        <button 
                            className="btn btn-danger"
                            onClick={() => {
                                localStorage.removeItem('token');
                                localStorage.removeItem('username');
                                actions.setIsAuthenticated(false);
                                actions.setCurrentPlayer("");
                            }}
                        >
                            Logout
                        </button>
                    </div>
                )}
            </div>

            {!session.isAuth ? (
                <div className="glass-panel" style={{ textAlign: 'center' }}>
                    <Auth onLoginSuccess={(username) => {
                        actions.setIsAuthenticated(true);
                        actions.setCurrentPlayer(username);
                    }} /> 
                    <div style={{ marginTop: '30px' }}>
                        <button className="btn btn-primary" onClick={() => actions.setIsAuthenticated(true)}>
                            Go to Game
                        </button>
                    </div>
                </div>
            ) : !room.isJoined ? (
                <div className="glass-panel" style={{ textAlign: 'center' }}>
                    <h2 className="subtitle">Join a Room</h2>
                    <input
                        type="text"
                        className="cyber-input"
                        value={room.code}
                        onChange={(e) => actions.setRoomCode(e.target.value.toUpperCase())}
                        placeholder="ENTER CODE"
                        style={{ width: '60%', margin: '20px auto', display: 'block',color:'#000',backgroundColor: '#FFF',  textAlign: 'center', fontSize: '24px', letterSpacing: '4px' }}
                    />
                    <button className="btn btn-primary" onClick={actions.handleJoinRooms} style={{ fontSize: '1.2rem', padding: '15px 40px' }}>
                        JOIN GAME
                    </button>
                </div>
            ) : game.status === 'lobby' ? (
                <div className="glass-panel" style={{ textAlign: 'center' }}>
                    <h2 className="subtitle" style={{ fontSize: '28px' }}>
                        Room: <span style={{ color: '#fff', letterSpacing: '2px' }}>{room.code}</span>
                    </h2>
                    <p style={{ color: '#888899', marginBottom: '30px' }}>Waiting for the race to start...</p>

                    <div style={{ display: 'flex', justifyContent: 'space-between', gap: '20px', marginBottom: '30px', flexWrap: 'wrap' }}>
                        
                        <div className="glass-panel-sm" style={{ flex: '1 1 300px', textAlign: 'left' }}>
                            <h3 style={{ color: '#00ff66', marginTop: 0, borderBottom: '1px solid #2a2a35', paddingBottom: '10px' }}>
                                Players ({room.players.length})
                            </h3>
                            <ul style={{ listStyleType: 'none', padding: 0, margin: 0, maxHeight: '150px', overflowY: 'auto' }}>
                                {room.players.length > 0 ? (
                                    room.players.map((p, index) => (
                                        <li key={index} style={{ padding: '8px 0', borderBottom: '1px solid #2a2a35', color: p === room.host ? '#ff9900' : '#e2e2e2', fontSize: '16px', display: 'flex', alignItems: 'center' }}>
                                            <span style={{ marginRight: '10px', fontSize: '12px', opacity: 0.8 }}>
                                                {p === room.host ? '[HOST]' : '[PILOT]'}
                                            </span>
                                            {p.length > 15 ? p.substring(0,15) + "..." : p}
                                        </li>
                                    ))
                                ) : (
                                    <li style={{ color: '#777', padding: '10px' }}>You are alone...</li>
                                )}
                            </ul>
                        </div>

                        <div className="glass-panel-sm" style={{ flex: '1 1 300px', textAlign: 'left' }}>
                            <h3 style={{ color: '#00f0ff', marginTop: 0, borderBottom: '1px solid #2a2a35', paddingBottom: '10px' }}>
                                Room Settings
                            </h3>
                            <div style={{ display: 'flex', flexDirection: 'column', gap: '15px' }}>
                                
                                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', color: '#e2e2e2' }}>
                                    <span>Power-Ups:</span>
                                    <label className="toggle-switch">
                                        <input 
                                            type="checkbox" 
                                            checked={room.settings?.powerUpsEnabled || false} 
                                            disabled={session.username !== room.host}
                                            onChange={(e) => actions.handleChangeSettings(e.target.checked, room.settings.hardMode, room.settings.secondsToEnd)}
                                        />
                                        <span className="slider"></span>
                                    </label>
                                </div>

                                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', color: '#e2e2e2' }}>
                                    <span>Hard Mode:</span>
                                    <label className="toggle-switch">
                                        <input 
                                            type="checkbox" 
                                            checked={room.settings?.hardMode || false} 
                                            disabled={session.username !== room.host}
                                            onChange={(e) => actions.handleChangeSettings(room.settings.powerUpsEnabled, e.target.checked, room.settings.secondsToEnd)}
                                        />
                                        <span className="slider"></span>
                                    </label>
                                </div>

                                <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', color: '#e2e2e2' }}>
                                    <span>Time Limit (s):</span>
                                    <input 
                                        type="number"
                                        className="cyber-input"
                                        min="0" max="300"
                                        value={room.settings?.secondsToEnd || 0} 
                                        disabled={session.username !== room.host}
                                        onChange={(e) => actions.handleChangeSettings(room.settings.powerUpsEnabled, room.settings.hardMode, Number(e.target.value) || 0)}
                                        style={{ width: '80px', padding: '8px', textAlign: 'center' }}
                                    />
                                </div>

                            </div>
                        </div>
                    </div>

                 {  /* <div style={{ marginBottom: '30px' }}>
                        <Chat 
                            currentPlayer={session.username} 
                            chatMessages={room.chat} 
                            sendChatMessage={actions.sendChatMessage} 
                        />
                    </div> */}

                    {session.username === room.host && (
                        <button className="btn btn-start-massive" onClick={actions.handleStart}>
                            Start Race
                        </button>
                    )}
                </div>
            ) : (
                <>
                    <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '20px', fontSize: '20px', fontWeight: 'bold' }}>
                        <div style={{ color: '#00f0ff', width: '33%' }}>{player.wpm} WPM</div>
                        <div style={{ color: '#ff9900', width: '33%', textAlign: 'center' }}>{computed.accuracy}%</div>
                        <div style={{ color: '#00ff66', width: '33%', textAlign: 'right' }}>{player.progress}%</div>
                    </div>

                    <div className="progress-track" style={{ marginBottom: '30px' }}>
                        <div className="progress-fill" style={{ width: `${player.progress}%` }} />
                    </div>

                    <div className="glass-panel-sm" style={{ marginBottom: '30px' }}>
                        <h3 style={{ color: '#888899', marginTop: 0, fontSize: '14px', textTransform: 'uppercase', letterSpacing: '1px', borderBottom: '1px solid #2a2a35', paddingBottom: '8px' }}>
                            Opponents
                        </h3>
                        {room.players.filter(p => p !== session.username).length > 0 ? (
                            room.players.filter(p => p !== session.username).map((opponentNick, index) => {
                                const stats = room.opponents?.[opponentNick] || { progress: 0, wpm: 0 };
                                return (
                                    <div key={index} style={{ marginBottom: '12px' }}>
                                        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', fontSize: '14px', color: '#e2e2e2', marginBottom: '5px' }}>
                                            <span style={{ fontWeight: 'bold' }}>{opponentNick}</span>
                                            <div style={{ display: 'flex', gap: '15px', alignItems: 'center' }}>
                                                {player.powerUp && game.status !== 'finished' && (
                                                    <button 
                                                        className="btn btn-primary"
                                                        onMouseDown={(e) => e.preventDefault()}
                                                        onClick={() => actions.handleUsePowerUp(opponentNick)}
                                                        style={{ padding: '4px 8px', fontSize: '12px', borderColor: '#bd00ff', color: '#bd00ff' }}
                                                    >
                                                        USE {player.powerUp.toUpperCase()}
                                                    </button>
                                                )}
                                                <span>{stats.wpm} WPM</span>
                                            </div>
                                        </div>
                                        <div className="progress-track" style={{ height: '6px' }}>
                                            <div className="progress-fill" style={{ width: `${stats.progress}%`, background: '#ff9900', boxShadow: '0 0 5px #ff9900' }} />
                                        </div>
                                    </div>
                                );
                            })
                        ) : (
                            <div style={{ color: '#777', fontSize: '14px', fontStyle: 'italic' }}>
                                You are racing against yourself!
                            </div>
                        )}
                    </div>

                    {game.countdown > 0 && (
                        <div style={{ textAlign: 'center', fontSize: '64px', color: '#ff9900', fontWeight: 'bold', marginBottom: '10px', textShadow: '0 0 20px rgba(255, 153, 0, 0.5)' }}>
                            {game.countdown}
                        </div>
                    )}
                    
                    {game.countdown === 0 && player.input.length === 0 && game.status !== 'finished' && (
                        <div style={{ textAlign: 'center', fontSize: '32px', color: '#00ff66', fontWeight: 'bold', marginBottom: '10px', height: '76px', display: 'flex', alignItems: 'center', justifyContent: 'center', textShadow: '0 0 20px rgba(0, 255, 102, 0.5)' }}>
                            START!
                        </div>
                    )}

                    {game.countdown === 0 && player.input.length > 0 && game.status !== 'finished' && (
                        <div style={{ height: '86px' }}></div>
                    )}
                    
                    {game.status === 'finished' && (
                        <div className="glass-panel" style={{ textAlign: 'center', padding: '20px', borderColor: '#00ff66' }}>
                            <h2 style={{ color: '#00ff66', margin: '0 0 10px 0', fontSize: '26px', textShadow: '0 0 10px rgba(0,255,102,0.4)' }}>
                                {game.winner === session.username ? "WIN!" : `WINNER: ${game.winner}`}
                            </h2>
                            <div style={{ fontSize: '18px', color: '#fff' }}>
                                Average: <span style={{color: '#00f0ff'}}>{player.wpm} WPM</span> | Accuracy: <span style={{color: '#ff9900'}}>{computed.accuracy}%</span>
                            </div>
                        </div>
                    )}

                    <div className={`text-display ${player.debuff === 'chaos' ? 'chaos-active' : ''}`}>
                        {actions.renderHighlightedText()}
                    </div>

                    {player.buff === 'shield' && game.status !== 'finished' && (
                        <div style={{ color: '#ffd700', fontSize: '18px', textAlign: 'center', fontWeight: 'bold', marginBottom: '15px', textTransform: 'uppercase', textShadow: '0 0 10px #ffd700', animation: 'fadeIn 0.3s ease-out' }}>
                            SHIELD ACTIVE! 
                        </div>
                    )}

                    {player.debuff && game.status !== 'finished' && (
                        <div style={{ color: '#ff007f', fontSize: '18px', textAlign: 'center', fontWeight: 'bold', marginBottom: '15px', textTransform: 'uppercase', textShadow: '0 0 15px rgba(255,0,127,0.6)', animation: 'fadeIn 0.3s ease-out' }}>
                            ATTACKED WITH: {player.debuff}
                        </div>
                    )}

                    <input
                        ref={inputRef}
                        type="text"
                        value={player.input}
                        onChange={actions.handleInputChange}
                        onKeyDown={actions.handleSpecialKeys}
                        disabled={game.status === 'finished' || game.countdown > 0 || player.debuff === 'freeze'}
                        className={`cyber-input race-input ${player.hasError ? 'error' : ''} ${player.buff === 'shield' ? 'shield-active' : ''} ${player.debuff === 'freeze' ? 'freeze-active' : ''}`}
                        style={{ 
                            opacity: game.countdown > 0 ? 0.5 : 1
                        }}
                        placeholder={game.countdown > 0 ? "Prepare..." : "Start"}
                    />

                    {game.status !== 'finished' && (
                        <>
                            <div className="progress-track" style={{ height: '6px', marginTop: '15px' }}>
                                <div
                                    className="progress-fill"
                                    style={{
                                        width: `${computed.powerUpProgress || 0}%`,
                                        background: player.powerUp ? '#bd00ff' : '#00f0ff',
                                        boxShadow: player.powerUp ? '0 0 15px #bd00ff' : '0 0 10px #00f0ff'
                                    }}
                                />
                            </div>
                            {player.powerUp && (
                                <div style={{ textAlign: 'center', color: '#bd00ff', fontSize: '14px', marginTop: '8px', fontWeight: 'bold', textTransform: 'uppercase', textShadow: '0 0 10px rgba(189,0,255,0.5)' }}>
                                    Power-up ready: {player.powerUp} (Press CTRL)
                                </div>
                            )}
                        </>
                    )}

                    {game.status === 'finished' && (
                        <>
                            <div style={{ marginTop: '30px', textAlign: 'center' }}>
                                {session.username === room.host && (
                                    <button className="btn btn-primary" onClick={actions.handleRestart} style={{ padding: '12px 30px', fontSize: '1.2rem' }}>
                                        PLAY AGAIN
                                    </button>
                                )}
                            </div>

                            {game.leaderboard.length > 0 && (
                                <div className="glass-panel" style={{ marginTop: '50px' }}>
                                    <h2 className="subtitle" style={{ textAlign: 'center', marginBottom: '20px' }}>TOP 10</h2>
                                    <table style={{ width: '100%', borderCollapse: 'collapse', color: '#e2e2e2', textAlign: 'left', fontSize: '16px' }}>
                                        <thead>
                                            <tr style={{ borderBottom: '2px solid #2a2a35' }}>
                                                <th style={{ padding: '12px' }}>#</th>
                                                <th style={{ padding: '12px' }}>Nickname</th>
                                                <th style={{ padding: '12px' }}>Played</th>
                                                <th style={{ padding: '12px' }}>Win Rate</th>
                                                <th style={{ padding: '12px' }}>Best WPM</th>
                                            </tr>
                                        </thead>
                                        <tbody>
                                            {game.leaderboard.map((p, index) => (
                                                <tr key={index} style={{ borderBottom: '1px solid rgba(42, 42, 53, 0.5)', backgroundColor: index % 2 === 0 ? 'rgba(0,0,0,0.2)' : 'transparent' }}>
                                                    <td style={{ padding: '12px', color: '#888899' }}>{index + 1}</td>
                                                    <td style={{ padding: '12px', fontWeight: 'bold', color: p.username === session.username ? '#00f0ff' : '#fff' }}>
                                                        {p.username}
                                                    </td>
                                                    <td style={{ padding: '12px' }}>{p.gamesPlayed}</td>
                                                    <td style={{ padding: '12px', color: '#ff9900' }}>{p.winrate}%</td>
                                                    <td style={{ padding: '12px', color: '#00ff66', fontWeight: 'bold' }}>{p.highScoreWpm}</td>
                                                </tr>
                                            ))}
                                        </tbody>
                                    </table>
                                </div>
                            )}
                        </>
                    )}
                </>
            )}
        </div>
    );
}

export default App;
