using Microsoft.AspNetCore.Http;
using TypeRacerServer.Core.Interfaces;
using TypeRacerServer.Core.Requests;
using TypeRacerServer.Core.ValueObjects;

namespace TypeRacerServer.Core.Services;


public class SaveScoreService(ISaveScoreRepository _repository)
{
    public async Task SaveScoreHandler(SaveScoreRequest saveScoreRequest, Username usernameFromToken)
    {
        if(string.IsNullOrEmpty(usernameFromToken)){
            throw new Exception("Invalid username");
        }
        var user = await _repository.getUser(usernameFromToken);
        if(user == null){
            throw new Exception("User not found");
        }
        user.GamesPlayed++;
        if(saveScoreRequest.isWinner){
            user.GamesWin++;
        }
        if (saveScoreRequest.WPM > user.HighScoreWpm){
            user.HighScoreWpm = saveScoreRequest.WPM;
        }

        await _repository.saveScore();
    }
}