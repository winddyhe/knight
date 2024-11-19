using Knight.Core;
using System.Collections.Generic;

namespace Game
{
    public class Model : IModel
    {
        public static Model Instance = Singleton<Model>.Instance;

        public List<IModel> Models;

        public PlayerModel Player;

        public Model()
        {
        }

        public void Initialize()
        {
            this.Models = new List<IModel>();

            this.Player = new PlayerModel();
            this.Models.Add(this.Player);

            foreach (var rModel in this.Models)
            {
                rModel.Initialize();
            }
        }

        public void Destroy()
        {
            foreach (var rModel in this.Models)
            {
                rModel.Destroy();
            }
            this.Models.Clear();
        }

        public void Login()
        {
            foreach (var rModel in this.Models)
            {
                rModel.Login();
            }
        }

        public void Logout()
        {
            foreach (var rModel in this.Models)
            {
                rModel.Logout();
            }
        }
    }
}
