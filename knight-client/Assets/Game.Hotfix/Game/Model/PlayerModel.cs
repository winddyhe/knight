using Game.Proto;
using Google.Protobuf;

namespace Game
{
    public class PlayerModel : ModelBase<PlayerViewModel>
    {
        public Player Player;

        public PlayerModel()
        {
        }

        protected override void OnLogin()
        {
            this.Player = null;
        }

        protected override void OnLogout()
        {
            this.Player = null;
        }

        [NetworkMessageHandler(GAME_CMD.GAME_CMD_LOGIN, GAME_CMD_LOGIN.GAME_CMD_LOGIN_PLAYER_DATA_S, typeof(Player))]
        private void OnLoginPlayerDataHandler(IMessage rMessage)
        {
            this.Player = ((Player)rMessage);
            this.SyncViewModels(rMessage);
        }
    }
}
