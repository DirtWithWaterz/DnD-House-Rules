using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEditor;
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

    InqueCalendar calendar;

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

    Dictionary<string, int> bodypartDict = new Dictionary<string, int>(){

        {"head", 0},
        {"neck", 1},
        {"chest", 2},
        {"leftArm", 3},
        {"leftForearm", 4},
        {"leftHand", 5},
        {"rightArm", 6},
        {"rightForearm", 7},
        {"rightHand", 8},
        {"torso", 9},
        {"pelvis", 10},
        {"leftThigh", 11},
        {"leftCrus", 12},
        {"leftFoot", 13},
        {"rightThigh", 14},
        {"rightCrus", 15},
        {"rightFoot", 16}
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

    IEnumerator Start(){

        SceneManager.activeSceneChanged += SceneChanged;

        yield return new WaitUntil(() => GameManager.Singleton != null);
        calendar = GameManager.Singleton.inqueCalendar;
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

            transform.root.GetChild(0).gameObject.SetActive(false);
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
            if(args[1] == "hp"){

                if(args[2] == "head"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].HP_HEAD.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "neck"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].HP_NECK.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "chest"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].HP_CHEST.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "leftArm"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].HP_ARM_LEFT.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "leftForearm"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].HP_FOREARM_LEFT.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "leftHand"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].HP_HAND_LEFT.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "rightArm"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].HP_ARM_RIGHT.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "rightForearm"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].HP_FOREARM_RIGHT.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "rightHand"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].HP_HAND_RIGHT.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "torso"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].HP_TORSO.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "pelvis"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].HP_PELVIS.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "leftThigh"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].HP_THIGH_LEFT.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "leftCrus"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].HP_CRUS_LEFT.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "leftFoot"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].HP_FOOT_LEFT.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "rightThigh"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].HP_THIGH_RIGHT.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "rightCrus"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].HP_CRUS_RIGHT.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "rightFoot"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].HP_FOOT_RIGHT.ToString());
                            return response;
                        }
                    }
                }
            }
            if(args[1] == "ac"){

                if(args[2] == "head"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].AC_HEAD.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "neck"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].AC_NECK.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "chest"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].AC_CHEST.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "leftArm"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].AC_ARM_LEFT.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "leftForearm"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].AC_FOREARM_LEFT.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "leftHand"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].AC_HAND_LEFT.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "rightArm"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].AC_ARM_RIGHT.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "rightForearm"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].AC_FOREARM_RIGHT.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "rightHand"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].AC_HAND_RIGHT.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "torso"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].AC_TORSO.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "pelvis"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].AC_PELVIS.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "leftThigh"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].AC_THIGH_LEFT.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "leftCrus"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].AC_CRUS_LEFT.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "leftFoot"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].AC_FOOT_LEFT.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "rightThigh"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].AC_THIGH_RIGHT.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "rightCrus"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].AC_CRUS_RIGHT.ToString());
                            return response;
                        }
                    }
                }
                if(args[2] == "rightFoot"){

                    string username = args[3];
                    for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                        if(GameManager.Singleton.userDatas[i].username == username){

                            response.Add(GameManager.Singleton.userDatas[i].AC_FOOT_RIGHT.ToString());
                            return response;
                        }
                    }
                }
            }
            if(args[1] == "lvl"){

                string username = args[2];
                for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                    if(GameManager.Singleton.userDatas[i].username == username){

                        response.Add(GameManager.Singleton.userDatas[i].Lvl.ToString());
                        return response;
                    }
                }
            }
            if(args[1] == "mod"){

                string username = args[3];
                for(int i = 0; i < GameManager.Singleton.userDatas.Count; i++){

                    if(GameManager.Singleton.userDatas[i].username == username){

                        switch(args[2]){
                            case "str":
                                response.Add(GameManager.Singleton.userDatas[i].Str.ToString());
                                break;
                            case "dex":
                                response.Add(GameManager.Singleton.userDatas[i].Dex.ToString());
                                break;
                            case "con":
                                response.Add(GameManager.Singleton.userDatas[i].Con.ToString());
                                break;
                            case "int":
                                response.Add(GameManager.Singleton.userDatas[i].Int.ToString());
                                break;
                            case "wis":
                                response.Add(GameManager.Singleton.userDatas[i].Wis.ToString());
                                break;
                            case "cha":
                                response.Add(GameManager.Singleton.userDatas[i].Cha.ToString());
                                break;
                            
                        }
                        return response;
                    }
                }
            }
            if(args[1] == "date"){

                response.Add($"Year: {calendar.currentYear.Value}");
                response.Add($"Month: {calendar.monthNames[calendar.currentMonth.Value]} ({calendar.currentMonth.Value}/{calendar.totalMonths})");
                response.Add($"Day: {calendar.currentMonthday.Value}{GetOrdinalSuffix(calendar.currentMonthday.Value)} ({calendar.currentMonthday.Value}/{calendar.totalMonthdays.Value})");
                // Combine current time into total minutes
                int currentTime = calendar.currentHour.Value * 60 + calendar.currentMin.Value;
                float sunriseTime = calendar.currentSunrise.x * 60 + calendar.currentSunrise.y;
                float sunsetTime = calendar.currentSunset.x * 60 + calendar.currentSunset.y;

                // Compare as single values
                string AmPm = (currentTime >= sunriseTime && currentTime <= sunsetTime) ? "am" : "pm";
                response.Add($"{calendar.currentHour.Value:00}:{calendar.currentMin.Value:00}{AmPm} ({calendar.totalHours + 1}hr clock)");
                response.Add("");
                calendar.GetCurrentSeason(out Season currentSeason, out Season previousSeason, out Season nextSeason);
                response.Add($"Season: {currentSeason.name}");
                response.Add($"Sunrise: {calendar.currentSunrise.x:00}:{calendar.currentSunrise.y:00}");
                response.Add($"Sunset: {calendar.currentSunset.x:00}:{calendar.currentSunset.y:00}");
                response.Add("");

                return response;
            }
            if(args[1] == "time"){

                // Combine current time into total minutes
                int currentTime = calendar.currentHour.Value * 60 + calendar.currentMin.Value;
                float sunriseTime = calendar.currentSunrise.x * 60 + calendar.currentSunrise.y;
                float sunsetTime = calendar.currentSunset.x * 60 + calendar.currentSunset.y;

                // Compare as single values
                string AmPm = (currentTime >= sunriseTime && currentTime <= sunsetTime) ? "am" : "pm";
                
                response.Add($"{calendar.currentHour.Value:00}:{calendar.currentMin.Value:00}{AmPm} ({calendar.totalHours + 1}hr clock)");
                response.Add("");
                response.Add($"Sunrise: {calendar.currentSunrise.x:00}:{calendar.currentSunrise.y:00}");
                response.Add($"Sunset: {calendar.currentSunset.x:00}:{calendar.currentSunset.y:00}");
                response.Add("");

                return response;
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
                transform.root.GetChild(0).gameObject.SetActive(false);
                response.Add("");
                return response;
            }
        }

        if(args[0] == "set"){

            if(args[1] == "ac"){

                if(IsHost){

                    SetArmorClassRpc(args[3], bodypartDict.GetValueOrDefault(args[2]), int.Parse(args[4]));
                }
                response.Add("");

                return response;
                
            }
            if(args[1] == "hp"){

                if(IsHost){

                    SetHealthRpc(args[3], bodypartDict.GetValueOrDefault(args[2]), int.Parse(args[4]));
                }
                response.Add("");

                return response;
                
            }
            if(args[1] == "lvl"){

                if(IsHost){

                    SetLevelRpc(args[2], int.Parse(args[3]));
                }
                response.Add("");

                return response;
            }
            if(args[1] == "mod"){

                if(IsHost){

                    SetModRpc(args[3], args[2], int.Parse(args[4]));
                }
                response.Add("");

                return response;
            }
            if(args[1] == "year"){
                if(IsHost)
                    SetTimeRpc(0, int.Parse(args[2]));
            }
            if(args[1] == "month"){

                if(IsHost)
                    SetTimeRpc(1, int.Parse(args[2]));
            }
            if(args[1] == "day"){

                if(IsHost)
                    SetTimeRpc(2, int.Parse(args[2]));
            }
            if(args[1] == "hour"){

                if(IsHost)
                    SetTimeRpc(3, int.Parse(args[2]));
            }
            if(args[1] == "min"){

                if(IsHost)
                    SetTimeRpc(4, int.Parse(args[2]));
            }
            if(args[1] == "time"){

                if(IsHost)
                    SetTimeRpc(5, 0, args[2]);
            }
            response.Add("");

            return response;
        }
        if(args[0] == "add" && IsHost){ // "add 2 years time"

            if(args[3] == "time"){

                switch(args[2]){

                    case "years":
                        int currentYear = calendar.currentYear.Value;
                        int yearDifference = int.Parse(args[1]);
                        int targetYear = currentYear + yearDifference;
                        SetTimeRpc(0, targetYear);
                        break;
                    case "months":
                        int currentMonth = calendar.currentMonth.Value;
                        int monthDifference = int.Parse(args[1]);
                        int targetMonth = currentMonth + monthDifference;

                        yearDifference = 0;

                        // Adjust for overflow or underflow of months
                        while (targetMonth > calendar.totalMonths) {
                            targetMonth -= calendar.totalMonths;
                            yearDifference++;
                        }
                        while (targetMonth <= 0) {
                            targetMonth += calendar.totalMonths;
                            yearDifference--;
                        }

                        // Update the year first, then the month
                        if (yearDifference != 0) {
                            currentYear = calendar.currentYear.Value;
                            SetTimeRpc(0, currentYear + yearDifference);
                        }
                        SetTimeRpc(1, targetMonth);
                        break;
                    case "days":
                        int currentDay = calendar.currentMonthday.Value;
                        int dayDifference = int.Parse(args[1]);
                        int targetDay = currentDay + dayDifference;

                        currentMonth = calendar.currentMonth.Value;
                        targetMonth = currentMonth;

                        // Move forward or backward through months
                        while (targetDay > calendar.monthDayNum[targetMonth]) {
                            targetDay -= calendar.monthDayNum[targetMonth];
                            targetMonth++;
                            if (targetMonth > calendar.totalMonths) { // Wraparound for months
                                targetMonth = 1;
                                SetTimeRpc(0, calendar.currentYear.Value + 1); // Increment year
                            }
                        }
                        while (targetDay <= 0) { // Move backward into previous months
                            targetMonth--;
                            if (targetMonth <= 0) { // Wraparound for months
                                targetMonth = calendar.totalMonths;
                                SetTimeRpc(0, calendar.currentYear.Value - 1); // Decrement year
                            }
                            targetDay += calendar.monthDayNum[targetMonth];
                        }

                        // Set month first, then day
                        SetTimeRpc(1, targetMonth);
                        SetTimeRpc(2, targetDay);
                        break;
                    case "hours":
                        int currentHour = calendar.currentHour.Value;
                        int hourDifference = int.Parse(args[1]);
                        int targetHour = currentHour + hourDifference;

                        dayDifference = 0;

                        // Handle overflow and underflow for hours
                        while (targetHour >= calendar.totalHours) {
                            targetHour -= calendar.totalHours;
                            dayDifference++;
                        }
                        while (targetHour < 0) {
                            targetHour += calendar.totalHours;
                            dayDifference--;
                        }

                        // Overflow into days, months, and years (recursive handling)
                        if (dayDifference != 0) {
                            currentDay = calendar.currentMonthday.Value;
                            targetDay = currentDay + dayDifference;

                            currentMonth = calendar.currentMonth.Value;
                            targetMonth = currentMonth;

                            // Move forward or backward through months
                            while (targetDay > calendar.monthDayNum[targetMonth]) {
                                targetDay -= calendar.monthDayNum[targetMonth];
                                targetMonth++;
                                if (targetMonth > calendar.totalMonths) { // Wraparound for months
                                    targetMonth = 1;
                                    SetTimeRpc(0, calendar.currentYear.Value + 1); // Increment year
                                }
                            }
                            while (targetDay <= 0) { // Move backward into previous months
                                targetMonth--;
                                if (targetMonth <= 0) { // Wraparound for months
                                    targetMonth = calendar.totalMonths;
                                    SetTimeRpc(0, calendar.currentYear.Value - 1); // Decrement year
                                }
                                targetDay += calendar.monthDayNum[targetMonth];
                            }

                            // Set month first, then day
                            SetTimeRpc(1, targetMonth);
                            SetTimeRpc(2, targetDay);
                        }

                        SetTimeRpc(3, targetHour); // Finally update the hours
                        break;
                    case "minutes":
                        int currentMinute = calendar.currentMin.Value;
                        int minuteDifference = int.Parse(args[1]);
                        int targetMinute = currentMinute + minuteDifference;

                        hourDifference = 0;

                        // Handle overflow and underflow for minutes
                        while (targetMinute >= calendar.totalMins + 1) {
                            targetMinute -= calendar.totalMins + 1;
                            hourDifference++;
                        }
                        while (targetMinute < 0) {
                            targetMinute += calendar.totalMins + 1;
                            hourDifference--;
                        }

                        // Overflow into hours, days, months, and years
                        if (hourDifference != 0) {
                            currentHour = calendar.currentHour.Value;
                            targetHour = currentHour + hourDifference;

                            dayDifference = 0;

                            // Handle overflow and underflow for hours
                            while (targetHour >= calendar.totalHours + 1) {
                                targetHour -= calendar.totalHours + 1;
                                dayDifference++;
                            }
                            while (targetHour < 0) {
                                targetHour += calendar.totalHours + 1;
                                dayDifference--;
                            }

                            // Handle days, months, and years recursively
                            if (dayDifference != 0) {
                                currentDay = calendar.currentMonthday.Value;
                                targetDay = currentDay + dayDifference;

                                currentMonth = calendar.currentMonth.Value;
                                targetMonth = currentMonth;

                                // Move forward or backward through months
                                while (targetDay > calendar.monthDayNum[targetMonth]) {
                                    targetDay -= calendar.monthDayNum[targetMonth];
                                    targetMonth++;
                                    if (targetMonth > calendar.totalMonths) { // Wraparound for months
                                        targetMonth = 1;
                                        SetTimeRpc(0, calendar.currentYear.Value + 1); // Increment year
                                    }
                                }
                                while (targetDay <= 0) { // Move backward into previous months
                                    targetMonth--;
                                    if (targetMonth <= 0) { // Wraparound for months
                                        targetMonth = calendar.totalMonths;
                                        SetTimeRpc(0, calendar.currentYear.Value - 1); // Decrement year
                                    }
                                    targetDay += calendar.monthDayNum[targetMonth];
                                }

                                // Set month first, then day
                                SetTimeRpc(1, targetMonth);
                                SetTimeRpc(2, targetDay);
                            }

                            SetTimeRpc(3, targetHour); // Update hours
                        }

                        SetTimeRpc(4, targetMinute);
                        break;
                }
            }
            response.Add("");
            return response;
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

    string GetOrdinalSuffix(int number) {
        // Handle special cases (11th, 12th, 13th)
        if (number % 100 >= 11 && number % 100 <= 13) {
            return "th";
        }

        // Handle general cases (1st, 2nd, 3rd, 4th, etc.)
        switch (number % 10) {
            case 1: return "st";
            case 2: return "nd";
            case 3: return "rd";
            default: return "th";
        }
    }

    // timeIndex (0 = year, 1 = month, 2 = day, 3 = hour, 4 = specific
    // specific = year:month:day:hour:minute

    [Rpc(SendTo.Server)]
    void SetTimeRpc(byte timeIndex, int val = 0, string data = ""){

        switch(timeIndex){

            case 0:
                
                int targetYear = val;
                int currentYear = calendar.currentYear.Value;

                // Calculate the difference between the target year and the current one, in years.
                int yearDifference = targetYear - currentYear;
                // Converts the above to days
                int dayDifference = yearDifference * (calendar.totalDays+1);

                // Update the weekday next
                calendar.currentWeekday.Value =
                    ((calendar.currentWeekday.Value + dayDifference - 1) % calendar.totalWeekdays) + 1;
                
                // Handle negative wrap-around for weekdays
                if(calendar.currentWeekday.Value <= 0)
                    calendar.currentWeekday.Value += calendar.totalWeekdays;

                calendar.currentDay.Value += dayDifference;
                calendar.currentYear.Value=targetYear;

                break;
            case 1:

                int targetMonth = val;
                int currentMonth = calendar.currentMonth.Value;

                // Calculate the difference between the target month and the current one, in months.
                int monthDifference = targetMonth - currentMonth;
                // Calculate the difference in days
                dayDifference = 0;
                for(int i = 0; i < Mathf.Abs(monthDifference); i++){

                    if(monthDifference < 0){

                        dayDifference -= calendar.monthDayNum[currentMonth - i - 1] - 1;
                    }
                    else{

                        dayDifference += calendar.monthDayNum[currentMonth + i] - 1;
                    }
                }

                // Set the day to the first of the month
                if(calendar.currentMonthday.Value > calendar.monthDayNum[targetMonth]){

                    dayDifference = dayDifference > 0 ? dayDifference - calendar.currentMonthday.Value + 5 : dayDifference - calendar.currentMonthday.Value + 1;
                    calendar.currentMonthday.Value = 1;
                }
                else{

                    dayDifference = dayDifference - calendar.currentMonthday.Value + 1;
                    calendar.currentMonthday.Value = 1;
                }

                // Update the weekday
                calendar.currentWeekday.Value =
                    (calendar.currentWeekday.Value + dayDifference - 1) % calendar.totalWeekdays + 1;
                // Handle negative wrap-around for weekdays
                if(calendar.currentWeekday.Value <= 0)
                    calendar.currentWeekday.Value += calendar.totalWeekdays;

                calendar.currentDay.Value += dayDifference;
                calendar.currentMonth.Value += monthDifference;
                calendar.currentYearday.Value += dayDifference;

                break;
            case 2:

                int targetDay = val; // Target day of the month
                int currentDay = calendar.currentMonthday.Value;

                // Ensure the target day is valid for the current month
                int maxDaysInMonth = calendar.monthDayNum[calendar.currentMonth.Value];
                targetDay = Mathf.Clamp(targetDay, 1, maxDaysInMonth); // Prevent invalid day input

                // Calculate the day difference
                dayDifference = targetDay - currentDay;

                // Move the current day directly
                calendar.currentDay.Value += dayDifference; 
                calendar.currentYearday.Value += dayDifference;
                calendar.currentMonthday.Value = targetDay; // Update the day of the month

                // Update the weekday based on the day difference
                calendar.currentWeekday.Value = 
                    ((calendar.currentWeekday.Value + dayDifference - 1) % calendar.totalWeekdays) + 1;

                // Handle negative wrap-around for weekdays
                if (calendar.currentWeekday.Value <= 0) {
                    calendar.currentWeekday.Value += calendar.totalWeekdays;
                }

                break;
            case 3:

                int targetHour = val;
                int currentHour = calendar.currentHour.Value;

                int maxHoursInDay = calendar.totalHours;
                targetHour = Mathf.Clamp(targetHour, 0, maxHoursInDay);

                int hourDifference = targetHour - currentHour;

                calendar.currentHour.Value += hourDifference;

                break;
            case 4:

                int targetMinute = val;
                int currentMinute = calendar.currentMin.Value;

                int maxMinutesInHour = calendar.totalMins;
                targetMinute = Mathf.Clamp(targetMinute, 0, maxMinutesInHour);

                int minuteDifference = targetMinute - currentMinute;

                calendar.currentMin.Value += minuteDifference;
                
                break;
            case 5:

                string[] args = data.Split(":");

                SetTimeRpc(0, int.Parse(args[0]));
                SetTimeRpc(1, int.Parse(args[1]));
                SetTimeRpc(2, int.Parse(args[2]));
                SetTimeRpc(3, int.Parse(args[3]));
                SetTimeRpc(4, int.Parse(args[4]));

                break;
        }
    }

    [Rpc(SendTo.Everyone)]
    void SetHealthRpc(string usernameI, int index, int val){

        if(username != usernameI)
            return;
        User user = GameObject.Find(usernameI).GetComponent<User>();
        user.bodyparts[index].hp.Value = val;
    }
    [Rpc(SendTo.Everyone)]
    void SetArmorClassRpc(string usernameI, int index, int val){

        if(username != usernameI)
            return;
        User user = GameObject.Find(usernameI).GetComponent<User>();
        user.bodyparts[index].ac.Value = val;
    }
    [Rpc(SendTo.Everyone)]
    void SetLevelRpc(string usernameI, int val){

        if(username != usernameI)
            return;
        User user = GameObject.Find(usernameI).GetComponent<User>();
        user.stats.lvl.Value = val;
    }
    [Rpc(SendTo.Everyone)]
    void SetModRpc(string usernameI, string mod, int val){

        if(username != usernameI)
            return;
        User user = GameObject.Find(usernameI).GetComponent<User>();
        switch(mod){

            case "str":
                user.stats.STR.Value = val;
                break;
            case "dex":
                user.stats.DEX.Value = val;
                break;
            case "con":
                user.stats.CON.Value = val;
                break;
            case "int":
                user.stats.INT.Value = val;
                break;
            case "wis":
                user.stats.WIS.Value = val;
                break;
            case "cha":
                user.stats.CHA.Value = val;
                break;
        }
    }

    [Rpc(SendTo.Server)]
    public void LoadGameSceneRpc(){

        NetworkManager.SceneManager.LoadScene("Game", UnityEngine.SceneManagement.LoadSceneMode.Single);
    }

    void SceneChanged(Scene arg0, Scene arg1){

        terminal.userInputLine.transform.SetAsLastSibling();
        terminal.userInputLine.SetActive(true);

        terminal.terminalInput.ActivateInputField();
        terminal.terminalInput.Select();
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
