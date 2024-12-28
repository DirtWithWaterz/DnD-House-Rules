using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Interpreter : NetworkBehaviour
{
    

    string joinCode = "";

    string resultStr = "Succeeded";

    string username = "unknown";

    public bool loggedIn;

    public User user;

    [SerializeField] TerminalManager terminal;

    [SerializeField] RelayManager relayManager;

    [SerializeField] PlayerSpawner playerSpawner;

    public Dictionary<string, string> colors = new Dictionary<string, string>(){

        {"black", "#021b21"},
        {"gray", "#555d71"},
        {"red", "#ff5879"},
        {"yellow", "#f2f1b9"},
        {"blue", "#9ed9d8"},
        {"purple", "#d926ff"},
        {"orange", "#ef5847"},
        {"white", "#FFFFFF"}
    };

    List<string> response = new List<string>();

    public string GetUsername{

        get{

            return username;
        }
        set{

            username = value;
        }
    }

    public List<string> Interpret(string userInput){

        response.Clear();

        string[] args = userInput.Split();

        if(args[0] == "StartUp" && !terminal.hasBooted){

            if(args[1] == "0"){

                response.Add("SOIB Date 07/13/05 " + DateTime.Now.ToString("hh:mm:ss"));
                return response;
            }
            if(args[1] == "1"){

                response.Add("CPU: letni(r) CPU 330 @ 40 MHz");
                return response;
            }
            if(args[1] == "2"){

                response.Add("Speed: 40 MHz");
                return response;
            }
            if(args[1] == "3"){

                response.Add("This AVG/ICP Soib is released under the UNG LPGL");
                return response;
            }
            if(args[1] == "4"){

                response.Add("");
                response.Add("Press F11 for SBB POPUP");
                return response;
            }
            if(args[1] == "5"){

                response.Add("Memory Clock: 64 MHz, Tcl:7 Trcd:4 Trp:8 Tras:20 (2T Timing) 8 bit");
                return response;
            }
            if(args[1] == "6"){

                response.Add("PMU ROM Version: 9303");
                return response;
            }
            if(args[1] == "7"){

                response.Add("NVMM ROM Version: 4.092.89");
                return response;
            }
            if(args[1] == "8"){

                response.Add("128MB OK");
                return response;
            }
            if(args[1] == "9"){

                response.Add("USB Device(s): 1 Keyboard, 1 Mouse, 1 Hub, 1 Storage Device");
                return response;
            }
            if(args[1] == "10"){

                response.Add("Auto-detecting USB Mass Storage Devices..");
                return response;
            }
            if(args[1] == "11"){

                response.Add("Device #01: USB 2.0 FlashDisk *Speed*");
                return response;
            }
            if(args[1] == "12"){

                response.Add("01 USB mass storage devices found and configured.");
                return response;
            }
            if(args[1] == "13"){

                response.Add("(C) Malato, Inc.");
                return response;
            }
            if(args[1] == "14"){

                response.Add("45-0010-00010-001010111-063606-79297-1AE0V003-Y2UC");
                return response;
            }
            if(args[1] == "15"){

                response.Add("Booting from Hard Disk...");
                return response;
            }
            if(args[1] == "16"){

                response.Add("");
                terminal.Clear();
                return response;
            }
            if(args[1] == "Help"){

                response.Add("Hello! Welcome to the Dungeon Master's terminal. To start hosting a server, ");
                response.Add("please input the following command: \"host\"");
                response.Add("to join an existing host, input: \"join [lobby code]\"");
                response.Add("for additional support input: \"help\"");
                response.Add("");
                response.Add(ColorString("before proceeding, please sign up and/or log in.", colors["yellow"]));
                response.Add("");
                response.Add("signup [username] [password] -- username must be more than 3 chars in length, ");
                response.Add("  and less than 20. Your password must be more than 8 chars in length, and ");
                response.Add("  less than 30, as well as must contain one lowercase, one uppercase, ");
                response.Add("  one number, and one symbol.");
                response.Add("login [username] [password] -- will log you into the specified account if it exists.");
                response.Add("");
                return response;
            }
        }

        if(args[0] == "help"){

            response.Add("quit -- quits the application. (there is no save function currently implemented)");
            response.Add("exit -- closes the terminal when in session.");
            response.Add("signup [username] [password] -- username must be more than 3 chars in length, ");
            response.Add("  and less than 20. Your password must be more than 8 chars in length, and ");
            response.Add("  less than 30, as well as must contain one lowercase, one uppercase, ");
            response.Add("  one number, and one symbol.");
            response.Add("login [username] [password] -- will log you into the specified account if it exists.");
            response.Add("host -- creates a new lobby for your players to join.");
            response.Add("join [lobby code] -- joins the lobby with the specified code, if one exists.");
            response.Add("list players -- list all players currently in the lobby.");
            response.Add("begin -- begins the session.");
            response.Add("save -- saves all players data to json files.");
            response.Add("clear -- clears the terminal.");
            response.Add("");

            return response;

        }

        if(args[0] == "quit"){

            response.Add("");
            Application.Quit();

            return response;
        }

        if(args[0] == "exit" && SceneManager.GetActiveScene().name == "Game"){

            transform.parent.gameObject.SetActive(false);
            user.screen.SetActive(true);
            response.Add("");
            return response;
        }

        if(args[0] == "signup"){

            try{

                SignUp(args[1], args[2]);
                response.Add("signup success. You are now logged in with the registered credentials.");
                username = args[1];
                return response;
            }
            catch(Exception ex){

                response.Add("signup failed with the following exception: ");
                response.Add(ex.ToString());
                return response;
            }
        }

        if(args[0] == "login"){

            try{

                LogIn(args[1], args[2]);
                response.Add($"login success. Welcome, {args[1]}.");
                username = args[1];
                return response;
            }
            catch(Exception ex){

                response.Add("login failed with the following exception: ");
                response.Add(ex.ToString());
                return response;
            }
        }

        if(args[0] == "host" && loggedIn){

            response.Add("Starting hosted session...");
            StartRelay();
            return response;
        }
        else if(args[0] == "host" && !loggedIn){

            response.Add("Please log in first.");
            return response;
        }

        if(args[0] == "lobbyCodeDisplayHidden()"){

            response.Add($"Result succeeded with the following lobby code: {joinCode}");
            response.Add("");
            return response;
        }
        if(args[0] == "display" && args[1] == "code"){

            response.Add(joinCode);
            response.Add("");
            return response;
        }

        if(args[0] == "joinAttemptDisplayHidden()"){

            response.Add($"result: {resultStr}");
            response.Add("");
            return response;
        }

        if(args[0] == "join" && loggedIn){

            response.Add("Starting client session...");
            try{

                response.Add($"Attemting to connect to host with lobby \"{args[1]}\"");
                JoinRelay(args[1]);
                response.Add("");
                return response;
            }
            catch{

                response.Add("You must input a lobby code.");
                response.Add("");
                return response;
            }
        }
        else if(args[0] == "join" && !loggedIn){

            response.Add("Please log in first.");
            return response;
        }

        if(args[0] == "list"){
            
            
            
            if(args[1] == "players"){

                foreach(userData data in GameManager.Singleton.userDatas){

                    response.Add($"{data.username}");
                }
                response.Add("");
                return response;
            }
            if(args[1] == "data"){

                
                if(args[2] == "chest"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].BODY_CHEST.ToString());
                            return response;
                        }
                    }
                    
                }
            }
            
        }

        if(args[0] == "begin"){

            if(!IsHost){
                
                response.Add("You do not have authority to start the session.");
                return response;
            }
            
            try{

                LoadGameSceneRpc();
                // transform.parent.gameObject.SetActive(false);
                response.Add("");
                return response;
            }
            catch(Exception ex){

                response.Add(ex.ToString());
                return response;
            }
        }

        if(args[0] == "open" && SceneManager.GetActiveScene().name == "Game"){

            if(args[1] == "sheet"){

                user.screen.SetActive(true);
                transform.parent.gameObject.SetActive(false);
                response.Add("");
                return response;
            }
        }

        if(args[0] == "clear")
        {
            terminal.Clear();
            return response;
        }

        else{

            response.Add($"'{string.Join(" ", args)}' is not recognized as a valid input,");
            response.Add("please try again.");
            response.Add("");

            return response;
        }
    }

    [Rpc(SendTo.Server)]
    public void LoadGameSceneRpc(){

        NetworkManager.SceneManager.LoadScene("Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    async void SignUp(string user, string pass){

        await relayManager.SignUpWithUsernamePasswordAsync(user, pass);
    }

    async void LogIn(string user, string pass){

        await relayManager.SignInWithUsernamePasswordAsync(user, pass);
    }

    async void StartRelay(){

        
        string joinCode = await relayManager.StartHostWithRelay();
        this.joinCode = joinCode;
        playerSpawner.OnIHateMyselfSpawnRpc();
        terminal.AddInterpreterLines(Interpret("lobbyCodeDisplayHidden()"));
        
        terminal.userInputLine.transform.SetAsLastSibling();
        terminal.userInputLine.SetActive(true);

        terminal.terminalInput.ActivateInputField();
        terminal.terminalInput.Select();

    }

    async void JoinRelay(string joinCode){

        bool success = await relayManager.StartClientWithRelay(joinCode);
        resultStr = success ? "Succeeded" : "Failed";
        this.joinCode = success ? joinCode : "You are not in a lobby.";
        terminal.AddInterpreterLines(Interpret("joinAttemptDisplayHidden()"));

        terminal.userInputLine.transform.SetAsLastSibling();
        terminal.userInputLine.SetActive(true);

        terminal.terminalInput.ActivateInputField();
        terminal.terminalInput.Select();

    }

    public string ColorString(string s, string color){

        string leftTag = $"<color={color}>";
        string rightTag = $"</color>";

        return $"{leftTag}{s}{rightTag}";
    }

    void ListEntry(string a, string b){

        response.Add($"{ColorString(a, colors["orange"])} : {ColorString(b, colors["yellow"])}");
    }
}
