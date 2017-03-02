using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;
using SharpDX;
using System;
using System.Linq;

namespace SmartCast
{
    internal class Program
    {
        private static Menu DevTools;

        private static void Main(string[] args)
        {
            Loading.OnLoadingComplete += OnLoadingComplete;
        }

        private static void OnLoadingComplete(EventArgs args)
        {
            DevTools = MainMenu.AddMenu("SmartCast", "DevTools", "SmartCast DevTools");

            foreach (AIHeroClient ally in EntityManager.Heroes.Allies)
            {
                DevTools.AddGroupLabel("[Ally] " + ally.BaseSkinName + " Position");
                DevTools.Add("[Ally]" + ally.BaseSkinName + "X", new Label("X Position: " + ally.Position.X));
                DevTools.Add("[Ally]" + ally.BaseSkinName + "Y", new Label("Y Position: " + ally.Position.Y));
                DevTools.Add("[Ally]" + ally.BaseSkinName + "Z", new Label("Z Position: " + ally.Position.Z));
            }

            foreach (AIHeroClient enemy in EntityManager.Heroes.Enemies)
            {
                DevTools.AddGroupLabel("[Enemy] " + enemy.BaseSkinName + " Position");
                DevTools.Add("[Enemy]" + enemy.BaseSkinName + "X", new Label("X Position: " + enemy.Position.X));
                DevTools.Add("[Enemy]" + enemy.BaseSkinName + "Y", new Label("Y Position: " + enemy.Position.Y));
                DevTools.Add("[Enemy]" + enemy.BaseSkinName + "Z", new Label("Z Position: " + enemy.Position.Z));
            }

            Obj_AI_Base.OnNewPath += OnNewPath;
            Chat.OnInput += OnInput;
        }

        private static void OnInput(ChatInputEventArgs args)
        {
            if (args.Input.ToLower().StartsWith("goto"))
            {
                string msg = args.Input;
                msg = msg.Replace("goto", "");
                msg = msg.Replace(" ", "");

                string[] value = msg.Split(',');

                if (value.Length != 2)
                    return;

                int x, y;
                bool xIsInteger = int.TryParse(value[0], out x);
                bool yIsInteger = int.TryParse(value[1], out y);

                if (!xIsInteger || !yIsInteger || x < 0 || x > 15000 || y < 0 || y > 15000)
                    return;

                Vector3 position = new Vector3(x, y, 0);
                args.Process = false;
                Orbwalker.MoveTo(position);
            }
        }

        private static void OnNewPath(Obj_AI_Base sender, GameObjectNewPathEventArgs args)
        {
            if (sender == null || args == null || args.Path == null || sender.Type != GameObjectType.AIHeroClient)
                return;

            string xPosition = "X Position: " + args.Path.LastOrDefault().X;
            string yPosition = "Y Position: " + args.Path.LastOrDefault().Y;
            string zPosition = "Z Position: " + args.Path.LastOrDefault().Z;

            if (sender.IsAlly)
            {
                DevTools.Get<Label>("[Ally]" + sender.BaseSkinName + "X").CurrentValue = xPosition;
                DevTools.Get<Label>("[Ally]" + sender.BaseSkinName + "Y").CurrentValue = yPosition;
                DevTools.Get<Label>("[Ally]" + sender.BaseSkinName + "Z").CurrentValue = zPosition;
            }
            else if (sender.IsEnemy)
            {
                DevTools.Get<Label>("[Enemy]" + sender.BaseSkinName + "X").CurrentValue = xPosition;
                DevTools.Get<Label>("[Enemy]" + sender.BaseSkinName + "Y").CurrentValue = yPosition;
                DevTools.Get<Label>("[Enemy]" + sender.BaseSkinName + "Z").CurrentValue = zPosition;
            }
        }
    }
}
