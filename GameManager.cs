/*
Project 0
24 September 2020

A simulation of the Prisoner's Dilemma.
*/

using System;
using System.Collections.Generic;
using System.Threading;

namespace Project_0{

    /// <summary>Class <c>GameManager</c> simulates the Prisoner's Dilemma game.</summary>
    public class GameManager{

        private const string HelpFile = "text\\help.txt";         // Path to help/info text file
        private const string IntroFile = "text\\intro.txt";       // Path to title text file
        private const string MenuFile = "text\\menu.txt";         // Path to menu text file
        private const string MissionFile = "text\\mission.txt";   // Path to mission description text file
        private const string NameFile = "text\\names.txt";        // Path to text file containing random names
        private const string ReplayFile = "text\\replay.txt";     // Path to replay text file
        private const string ResultFile = "text\\result.txt";     // Path to text file containing mission results
        private const string InputError = "Invalid input received.\n";  // Shown when invalid choice is made
        private const int Candidates = 3;                               // Number of candidates to choose from
        
        private bool _shouldReplay = true;              // Replay control variable
        private bool _playerConfessed;                  // The player's confession variable
        private Assistant _playerAssistant;             // The player's computer Assistant
        private List<Assistant> _assistants;            // List of Assistant candidates

        public bool ShouldReplay{
            get => _shouldReplay;
            set => _shouldReplay = value;
        }

        public bool PlayerConfessed{
            get => _playerConfessed;
            set => _playerConfessed = value;
        }

        public Assistant PlayerAssistant{
            get => _playerAssistant;
            set => _playerAssistant = value;
        }

        public List<Assistant> Assistants{
            get => _assistants;
            set => _assistants = value;
        }

        static void Main(string[] args){
            GameManager game = new GameManager();
            while(game.ShouldReplay){
                game.Init();
                game.Run();
            }
        }

        /// <summary>Method <c>Init</c> initializes the game's Assistant objects.</summary>
        public void Init(){
            Assistants = new List<Assistant>();
            for(int x = 0; x < Candidates; x++)
                _assistants.Add(new Assistant(PickName(), x+1, GenerateReliability()));
        }

        /// <summary>Method <c>Run</c> begins main game logic.</summary>
        public void Run(){
            ReadFile(IntroFile);
            Thread.Sleep(2000);
            ShowMenu();
            ShowTestMission();
            ShowAssistantChoice();
            ShowRealMission();
            Thread.Sleep(5000);
            ShowReplay();
        }

        // === Begin Game Methods ===
        /// <summary>Method <c>EvaluateConfessions</c> checks the player's confession status
        /// against the Assistant's.</summary>
        private void EvaluateConfessions(){
            bool assistantConfessed = PlayerAssistant.Confession();
            string delimiter = "#";
            if(PlayerConfessed && assistantConfessed)
                ReadFromPoint("1", delimiter);
            else if(PlayerConfessed && !assistantConfessed)
                ReadFromPoint("2", delimiter);
            else if(!PlayerConfessed && assistantConfessed)
                ReadFromPoint("3", delimiter);
            else
                ReadFromPoint("4", delimiter);
        }

        /// <summary>Method <c>ShowAssistantChoice</c> displays candidates to choose for Assistant.</summary>
        private void ShowAssistantChoice(){
            string input;
            while(true){
                Console.WriteLine("\n\tNow, select an Assistant for your Real Mission.\n");
                input = ProcessInput();
                if(input.Equals("1")){
                    PlayerAssistant = Assistants[0];
                    break;
                }else if(input.Equals("2")){
                    PlayerAssistant = Assistants[1];
                    break;
                }else if(input.Equals("3")){
                    PlayerAssistant = Assistants[2];
                    break;
                }else Console.WriteLine(InputError);
            }
        }

        /// <summary>Method <c>ShowMenu</c> displays the main menu.</summary>
        private void ShowMenu(){
            string input; 
            while(true){
                ReadFile(MenuFile);
                input = ProcessInput();
                if(input.Equals("1")){
                    break;
                }else if(input.Equals("2")){
                    ReadFile(HelpFile);
                }else if(input.Equals("3")){
                    System.Environment.Exit(1);
                    break;
                }else Console.WriteLine(InputError);
            }   
        }

        /// <summary>Method <c>ShowRealMission</c> displays and runs the Real Mission.</summary>
        private void ShowRealMission(){
            string input;
            Console.WriteLine("\tYou have selected "+PlayerAssistant.Name.Trim()+" as your Assistant.\n");
            Console.WriteLine("\tPress any key to begin the Real Mission.");
            Console.ReadKey();
            Standby();
            ReadFile(MissionFile);
            while(true){
                input = ProcessInput();
                if(input.Equals("1")){
                    PlayerConfessed = true;
                    break;
                }else if(input.Equals("2")){
                    PlayerConfessed = false;
                    break;
                }else Console.WriteLine(InputError);
            }
            Console.WriteLine("\tAfter leaving the room for a few minutes your captors return.");
            Standby();
            EvaluateConfessions();
        }

        /// <summary>Method <c>ShowReplay</c> displays replay menu.</summary>
        private void ShowReplay(){
            string input;
            Console.WriteLine("\tHere are the Reliability Ratings for each of the candidates:\n");
            PrintAssistants(true);
            Console.WriteLine();
            ReadFile(ReplayFile);
            while(true){
                input = ProcessInput();
                if(input.Equals("1")){
                    break;
                }else if(input.Equals("2")){
                    ShouldReplay = false;
                    break;
                }else Console.WriteLine(InputError);
            }
        }

        /// <summary>Method <c>ShowTestMission</c> displays and runs the Test Mission.</summary>
        private void ShowTestMission(){
            string action = "NO CONFESSION";
            Console.WriteLine("\tThe following are your candidates for this session:\n");
            PrintAssistants(false);
            Console.WriteLine("\n\tPress any key to begin the Test Mission.");
            Console.ReadKey();
            Standby();
            Console.WriteLine("\tTest Mission Results:\n");
            foreach(Assistant a in Assistants){
                if(a.Confession()) action = "CONFESSION";
                Console.WriteLine("\t"+a.Number+") "+a.Name+"\t Action: "+action);
            }
        }
        // === End Game Methods ===

        // === Begin Utility Methods ===
        /// <summary>Method <c>GenerateReliability</c> picks a random reliability rating.</summary>
        /// <returns>A float value between 0.0 and 1.0.</returns>
        private float GenerateReliability(){
            Random rand = new Random();
            return (float)rand.NextDouble();
        }

        /// <summary>Method <c>PickName</c> selects a name at random from a text file.</summary>
        /// <returns>A string containing the selected name.</returns>
        private string PickName(){
            string[] lines = System.IO.File.ReadAllLines(NameFile);
            int size = lines.Length;
            Random rand = new Random();
            return lines[rand.Next(0,size)];
        }

        /// <summary>Method <c>PrintAssistants</c> prints the List of Assistants.</summary>
        private void PrintAssistants(bool showReliability){
            string reliabilityValue = "", tab = "";
            foreach(Assistant a in Assistants){
                if(showReliability) reliabilityValue = "\tReliability: " + a.Reliability.ToString();
                if(a.Name.Length < 4) tab = "\t";
                Console.WriteLine("\t" + a.Number + ") " + a.Name + tab + reliabilityValue);
            }
        }

        /// <summary>Method <c>ProcessInput</c> takes user input from Terminal.</summary>
        /// <returns>A string containing the user input.</returns>
        private string ProcessInput(){
            Console.Write(">> ");
            return Console.ReadLine();
        } 

        /// <summary>Method <c>ReadFile</c> prints the contents of a text file.</summary>
        private void ReadFile(string filePath){
            string[] lines = System.IO.File.ReadAllLines(filePath);
            foreach(string line in lines)
                Console.WriteLine("\t" + line);
            Console.WriteLine();
        }

        /// <summary>Method <c>ReadFromPoint</c> prints the contents of a text file at a certain
        /// point denoted by the passed point value (single characters only). 
        /// Stops at passed delimiter value (single characters only).</summary>
        private void ReadFromPoint(string point, string delimiter){
            string line;
            System.IO.StreamReader file = new System.IO.StreamReader(ResultFile);
            while((line = file.ReadLine()) != null){
                if(line.Trim().Equals(point)){
                    line = file.ReadLine();
                    while(!line.Equals(delimiter)){   
                        Console.WriteLine("\t" + line);
                        line = file.ReadLine();
                    }
                    break;
                }
            }
            Console.WriteLine();
        }

        /// <summary>Method <c>Standby</c> simulates loading time.</summary>
        private void Standby(){
            int count = 0;
            Console.Write("\n\t");
            while(count < 10){
                Console.Write(". ");
                Thread.Sleep(500);
                count++;
            }
            Console.WriteLine("\n");
        }
        // === End Utility Methods ===

        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++
        // +++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        /// <summary>Class <c>Assistant</c> controls Assistant behavior during the game.</summary>
        public class Assistant{

            private string _name;
            private int _number;
            private float _reliability;

            public string Name{
                get => _name;
                set => _name = value;
            }

            public int Number{
                get => _number;
                set => _number = value;
            }

            public float Reliability{
                get => _reliability;
                set => _reliability = value;
            }
            
            public Assistant(string name, int number, float reliability){
                Name = name;
                Number = number;
                Reliability = reliability;
            }

            /// <summary>Method <c>Confession</c> randomly determines if the Assistant will confess.</summary>
            /// <returns>A Boolean value representing the confession.</returns>
            public bool Confession(){
                Random rand = new Random();
                float confessValue = (float)rand.NextDouble();
                return Reliability < confessValue;
            }
        }
    }
}