using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using Microsoft.AspNetCore.Authorization;

namespace TypeRacerServer;

public class RoomState
{
    public string TargetText { get; set; } = string.Empty;
    public ConcurrentDictionary<string, string> Players { get; set; } = new();
    public bool GameStarted { get; set; } = false;
    public string HostConnection { get; set; } = string.Empty;
    public bool PowerUpsEnabled { get; set; } = true;
    public int SecondsToEnd { get; set; } = 0;
    public bool HardMode { get; set; } = false;
    public Guid CurrentGameId { get; set; } = Guid.NewGuid();
}

public class PlayerSession
{
    public string Nickname { get; set; } = string.Empty;
    public string RoomCode { get; set; } = string.Empty;
    public string TargetText { get; set; } = string.Empty;
    
    public int Progress { get; set; }
    public int PowerUpProgress { get; set; }
    public int Keystrokes { get; set; }
    public int Errors { get; set; }
    public int DebuffsReceived { get; set; }
    
    public DateTime? StartTime { get; set; }
    public DateTime? FinishTime { get; set; }
    public DateTime? FreezeEnd { get; set; }

    public double Accuracy => Keystrokes > 0 ? ((double)(Keystrokes - Errors) / Keystrokes) * 100.0 : 0;
}

[Authorize]

public class GameHub : Hub
{
    private readonly IHubContext<GameHub> _hubContext;

    public GameHub(IHubContext<GameHub> hubContext)
    {
        _hubContext = hubContext;
    }
    
    private static readonly string[] Quotes = new[]
    {
        "public static void Main(string[] args) { Console.WriteLine(\"System Init\"); }",
        "var user = await db.Users.FirstOrDefaultAsync(u => u.Username == request.Username);",
        "public IEnumerable<int> GetNumbers() { yield return 1; }",
        "Task.Run(() => ProcessDataAsync(token)).Wait();",
        "[ApiController] [Route(\"api/[controller]\")] public class AuthController : ControllerBase { }",
        "private static ConcurrentDictionary<string, string> PlayerRooms = new();",
        "string newCid = Context.ConnectionId; var room = RoomManager.GetOrAdd(HostCode, _ => new RoomState());",
        "public async Task StartRoomGame(string HostCode) { room.GameStarted = true; }",
        "await Clients.Group(hostcode).SendAsync(\"UpdatePlayersList\", roomdata);",
        "if (DateTime.Now < freezeEnd) { return; } else { FrozenPlayers.TryRemove(cid, out _); }",
        "_ = Task.Run(async () => { await Task.Delay(15000); });",
        "bool isError = currentInput.Length > correctLength;",
        "int progressPercentage = (correctLength * 100) / targetText.Length;",
        "builder.Services.AddSignalR(); app.MapHub<GameHub>(\"/gamehub\");",
        "const hack = async (target) => { await sys.Bypass(target); return true; };",
        "document.getElementById(\"btn\").addEventListener(\"click\", () => alert(\"Hacked!\"));",
        "setTimeout(() => { console.log(\"Timeout finished\"); }, 1000);",
        "const [count, setCount] = useState(0); useEffect(() => { setCount(c => c + 1); }, []);",
        "import { HubConnectionBuilder } from '@microsoft/signalr';",
        "export const useGameLogic = () => { const [session, setSession] = useState(null); };",
        "const savedData = JSON.parse(localStorage.getItem('user_data')) || {};",
        "app.use((err, req, res, next) => { res.status(500).send('Server Error'); });",
        "const SECURE_KEY = crypto.createHash('sha256').update(pwd).digest('hex');",
        "SELECT * FROM Users WHERE clearance_level = 'TOP_SECRET' ORDER BY id DESC;",
        "UPDATE Players SET score = score + 100 WHERE username = 'admin';",
        "DELETE FROM system_logs WHERE date < NOW() - INTERVAL '7 days';",
        "INSERT INTO audit (event, ts) VALUES ('BREACH', CURRENT_TIMESTAMP);",
        "CREATE INDEX idx_user_email ON Users (email);",
        "def inject_payload(buffer): return buffer ^ 0xFF",
        "with open(\"config.json\", \"r\") as file: data = json.load(file)",
        "import numpy as np; arr = np.zeros((10, 10))",
        "@app.route('/api/v1/data', methods=['GET', 'POST'])",
        "class NeuralNet(nn.Module): def __init__(self): super().__init__()",
        "int main(int argc, char *argv[]) { return 0; }",
        "void* ptr = malloc(sizeof(int) * 100); free(ptr);",
        "std::cout << \"Segmentation fault (core dumped)\" << std::endl;",
        "for(int i=0; i<MAX_BUFFER; ++i) { buffer[i] = 0x00; }",
        "<div className=\"glass-panel\" style={{ textAlign: 'center' }}></div>",
        "body { background-color: var(--bg-main); font-family: 'Inter', sans-serif; }",
        "@keyframes glitch { 0% { transform: skewX(0deg); } 100% { transform: skewX(2deg); } }",
        "<input type=\"text\" className=\"cyber-input\" placeholder=\"ENTER CODE\" />",
        "chmod +x deploy.sh && ./deploy.sh --force",
        "ssh root@192.168.1.10 -p 2222 -i ~/.ssh/id_rsa",
        "tail -f /var/log/syslog | grep \"CRITICAL ERROR\"",
        "ping 127.0.0.1 -c 4 > /dev/null 2>&1",
        "curl -X POST http://localhost:5293/api/save-score -d '{\"Wpm\": 120}'",
        "tar -czvf backup.tar.gz /var/www/html/",
        "try { server.Overload(); } catch (Exception ex) { Protocol.InitiateWipe(); }",
        "if (enemy.Shields <= 0) { weapon.Fire(WeaponType.Laser); }",
        "sys.InitNetwork(); firewall.Bypass(Proxy.GenerateIP());",
        "void TriggerOverload() { core.Temp += 500; core.Melt(); }",
        "Connection.Intercept(); DataStream.Decrypt();"
    };

    private static readonly ConcurrentDictionary<string, RoomState> Rooms = new();
    private static readonly ConcurrentDictionary<string, PlayerSession> Sessions = new();

    private static readonly string[] Powers = new[] { "freeze", "flashbang", "chaos", "bomb" };
    private static readonly string[] Buffs = new[] { "shield" };

    private string GetRandomQuote() => Quotes[new Random().Next(Quotes.Length)];
    private string GetRandomBuff() => Buffs[new Random().Next(Buffs.Length)];
    private string GetRandomPower() => Powers[new Random().Next(Powers.Length)];

    public async Task<bool> JoinRoom(string HostCode)
    {
    	string currentPlayer = Context.User?.Identity?.Name ?? "Unkown";
        string newCid = Context.ConnectionId;
        var room = Rooms.GetOrAdd(HostCode, _ => new RoomState());

        var oldSessionEntry = Sessions.FirstOrDefault(s => s.Value.Nickname == currentPlayer && s.Value.RoomCode == HostCode);
        if (oldSessionEntry.Key != null)
        {
            string oldCid = oldSessionEntry.Key;
            var sessionData = oldSessionEntry.Value; 

            Sessions.TryRemove(oldCid, out _);
            Sessions.TryAdd(newCid, sessionData); 
            
            room.Players.TryRemove(oldCid, out _);
            if (room.HostConnection == oldCid) room.HostConnection = newCid;
        }

        if (!Sessions.ContainsKey(newCid))
        {
            Sessions.TryAdd(newCid, new PlayerSession { Nickname = currentPlayer, RoomCode = HostCode });
        }

        if (room.Players.Count == 0)
        {
            room.HostConnection = newCid;
            await Clients.Caller.SendAsync("SetUpLobby", new { PowerUpsEnabled = false, SecondsToEnd = 0, HardMode = false });
        }
        
        if (room.GameStarted && oldSessionEntry.Key == null) return false; 

        await Groups.AddToGroupAsync(newCid, HostCode);
        room.Players.TryAdd(newCid, currentPlayer);
        
        var roomdata = new { Players = room.Players.Values.ToArray(), Host = room.Players.GetValueOrDefault(room.HostConnection, "") };
        
        await Clients.Caller.SendAsync("UpdatePlayersList", roomdata);
        await Clients.GroupExcept(HostCode, newCid).SendAsync("UpdatePlayersList", roomdata);

        if (room.GameStarted && Sessions.TryGetValue(newCid, out var pSession) && !string.IsNullOrEmpty(pSession.TargetText))
        {
            await Clients.Caller.SendAsync("LoadText", pSession.TargetText);
        }

        return true;
    }

    public async Task StartRoomGame(string HostCode)
    {
        if (!Rooms.TryGetValue(HostCode, out var room) || Context.ConnectionId != room.HostConnection || room.GameStarted) 
            return;
        
        room.TargetText = GetRandomQuote();
        room.CurrentGameId = Guid.NewGuid(); 
        room.GameStarted = true;

        foreach (var cid in room.Players.Keys)
        {
            if (Sessions.TryGetValue(cid, out var player))
            {
                player.TargetText = room.TargetText;
                player.Progress = 0;
                player.Errors = 0;
                player.Keystrokes = 0;
                player.DebuffsReceived = 0;
                player.FinishTime = null;
                player.StartTime = null;
                player.PowerUpProgress = 0;
            }
        }
        await Clients.Group(HostCode).SendAsync("LoadText", room.TargetText);
    }

    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string cid = Context.ConnectionId;
        
        if (Sessions.TryGetValue(cid, out var player) && Rooms.TryGetValue(player.RoomCode, out var room))
        {
            _ = Task.Run(async () => 
            {
                await Task.Delay(15000);
                if (room.Players.ContainsKey(cid)) PerformCleanup(cid, player.RoomCode, player.Nickname, room);
            });
            
            await base.OnDisconnectedAsync(exception);
            return; 
        }
        
        PerformCleanup(cid, null, null, null);
        await base.OnDisconnectedAsync(exception);    
    }

    private void PerformCleanup(string cid, string? hostCode, string? nickname, RoomState? room)
    {
        Sessions.TryRemove(cid, out _);

        if (room != null && hostCode != null && nickname != null)
        {
            room.Players.TryRemove(cid, out _);

            if (room.HostConnection == cid)
                room.HostConnection = room.Players.Keys.FirstOrDefault() ?? string.Empty;

            if (room.Players.Count > 0)
            {
                var roomdata = new { Players = room.Players.Values.ToArray(), Host = room.Players.GetValueOrDefault(room.HostConnection, "") };
                _hubContext.Clients.Group(hostCode).SendAsync("UpdatePlayersList", roomdata);
                
                if (room.GameStarted)
                {
                    _hubContext.Clients.Group(hostCode).SendAsync("UpdateState", new {
                        playerNick = nickname, progress = 0, hasError = true, isDone = true, wpm = 0
                    });
                }
            }
            else Rooms.TryRemove(hostCode, out _);
        }
    }

    public async Task SendProgress(string currentInput)
    {
        string cid = Context.ConnectionId;

        if (!Sessions.TryGetValue(cid, out var player) || !Rooms.TryGetValue(player.RoomCode, out var room)) return;
        if (player.FreezeEnd.HasValue && DateTime.Now < player.FreezeEnd) return;
        if (player.FinishTime.HasValue) return;
        if (string.IsNullOrEmpty(player.TargetText)) return;

        player.FreezeEnd = null;
        player.Keystrokes++;

        if (currentInput.Length == 1 && !player.StartTime.HasValue)
            player.StartTime = DateTime.Now;

        int correctLength = 0;
        while (correctLength < currentInput.Length && correctLength < player.TargetText.Length && currentInput[correctLength] == player.TargetText[correctLength])
        {
            correctLength++;
        }
        
        bool isError = currentInput.Length > correctLength;
        if (isError) player.Errors++;

        player.Progress = (correctLength * 100) / player.TargetText.Length;

        if (player.PowerUpProgress < correctLength)
        {
            player.PowerUpProgress = correctLength;
            if (room.PowerUpsEnabled)
            {
                if (correctLength % 5 == 0) await Clients.Caller.SendAsync("PowerUpGranted", GetRandomPower());
                if (correctLength % 10 == 0) await Clients.Caller.SendAsync("ReceiveDefense", "auto", GetRandomBuff());
            }
        }

        if (isError && room.HardMode)
        {
            player.TargetText = string.Empty;
            await Clients.Group(player.RoomCode).SendAsync("ReceiveAttack", player.Nickname, "freeze");
            await Clients.Group(player.RoomCode).SendAsync("UpdateState", new {
                playerNick = player.Nickname, progress = player.Progress, hasError = true, isDone = true, wpm = 0
            });

            bool anyoneAlive = room.Players.Keys.Any(pId => Sessions.TryGetValue(pId, out var s) && !string.IsNullOrEmpty(s.TargetText) && !s.FinishTime.HasValue);
            if (!anyoneAlive && room.GameStarted) await EndGameProcess(player.RoomCode, room);
            return;
        }

        if (currentInput == player.TargetText && room.GameStarted)
        {    
            player.FinishTime = DateTime.Now; 

            int activePlayers = room.Players.Keys.Count(p => Sessions.TryGetValue(p, out var s) && !string.IsNullOrEmpty(s.TargetText));
            int finishedCount = room.Players.Keys.Count(p => Sessions.TryGetValue(p, out var s) && s.FinishTime.HasValue);

            if (finishedCount == 1)
            {
                if (room.SecondsToEnd > 0)
                {
                    Guid thisGameId = room.CurrentGameId;
                    _ = Task.Run(async () => {
                        await Task.Delay(room.SecondsToEnd * 1000);
                        if (Rooms.TryGetValue(player.RoomCode, out var r) && r.GameStarted && r.CurrentGameId == thisGameId)
                            await EndGameProcess(player.RoomCode, r);
                    });
                }
                else await EndGameProcess(player.RoomCode, room);
            }
            else if (finishedCount >= activePlayers)
            {
                await EndGameProcess(player.RoomCode, room);
            }
        }

        int currentWpm = 0;
        if (player.StartTime.HasValue)
        {
            var elapsedSeconds = (DateTime.Now - player.StartTime.Value).TotalSeconds;
            if (elapsedSeconds > 0) currentWpm = (int)((correctLength / 5.0) / (elapsedSeconds / 60.0));
        }

        if(currentWpm > 300) return;

        await Clients.Group(player.RoomCode).SendAsync("UpdateState", new {
            playerNick = player.Nickname, progress = player.Progress, hasError = isError, isDone = player.FinishTime.HasValue, wpm = currentWpm 
        });
    }

    public async Task UsePowerUp(string roomCode, string AttackerNick, string TargetNick, string Power)
    {
        if (Rooms.TryGetValue(roomCode, out var room) && room.GameStarted)
        {
            var targetCid = room.Players.FirstOrDefault(x => x.Value == TargetNick).Key;
            if (targetCid != null && Sessions.TryGetValue(targetCid, out var targetPlayer))
            {
                targetPlayer.DebuffsReceived++;
                if (Power == "freeze") targetPlayer.FreezeEnd = DateTime.Now.AddSeconds(3);
            }
            await Clients.Group(roomCode).SendAsync("ReceiveAttack", TargetNick, Power);
        }
    }

    public async Task SendMessage(string roomCode, string username, string messageText)
    {
        if (!string.IsNullOrWhiteSpace(messageText))
            await Clients.Group(roomCode).SendAsync("ReceiveChatMessage", username, messageText);
    }
    
    public async Task RestartGame()
    {
        string cid = Context.ConnectionId;
        if (Sessions.TryGetValue(cid, out var session) && Rooms.TryGetValue(session.RoomCode, out var room) && cid == room.HostConnection)
        {
            room.GameStarted = false;
            foreach (var pid in room.Players.Keys)
            {
                if (Sessions.TryGetValue(pid, out var p))
                {
                    p.StartTime = null;
                    p.TargetText = string.Empty;
                    p.PowerUpProgress = 0;
                    p.FinishTime = null;
                }
            }
            await Clients.Group(session.RoomCode).SendAsync("BackToLobby");
        }
    }

    public async Task ChangeRoomSettings(string roomCode, bool powerUpsEnabled, bool hardMode, int secondsToEnd)
    {
        if (Rooms.TryGetValue(roomCode, out var room) && room.HostConnection == Context.ConnectionId)
        {
            room.PowerUpsEnabled = powerUpsEnabled;
            room.HardMode = hardMode;
            room.SecondsToEnd = secondsToEnd;

            await Clients.Group(roomCode).SendAsync("SettingsUpdate", new {
                powerUpsEnabled, hardMode, secondsToEnd    
            });
        }
    }

    private async Task EndGameProcess(string hostCode, RoomState room)
    {
        room.GameStarted = false;
        
        var playersList = room.Players.Keys
            .Select(cid => Sessions.GetValueOrDefault(cid))
            .OfType<PlayerSession>() 
            .ToList();

        var sorted = playersList
            .OrderByDescending(p => p.Progress)
            .ThenBy(p => p.FinishTime ?? DateTime.MaxValue) 
            .ThenByDescending(p => p.Accuracy)              
            .ThenByDescending(p => p.DebuffsReceived)       
            .ThenBy(p => p.Nickname)                        
            .ThenBy(p => Guid.NewGuid())                    
            .ToList();

        string winnerNick = sorted.FirstOrDefault()?.Nickname ?? "None";
        await _hubContext.Clients.Group(hostCode).SendAsync("GameOver", winnerNick);
    }
}
