using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Mirror;

// Command classes
[System.Serializable]
public class DirectionCommand
{
    // Direction to move
    public string directionName;
    // Command to type to go that direction
    public string command;
    // Direction to go
    public Vector2 direction;
}

[System.Serializable]
public class AttackCommand
{
    // Name of command 
    public string commandName;
    // Command to type to do command
    public string command;

    // Whether a command relies on a direction command or not to work
    public enum DirectionType 
    {
        MustInclude,
        MustNotInclude,
        Optional
    };

    public DirectionType directionType;

    [System.NonSerialized]
    public CMDAttack attackScript;
}

public class PlayerCommands : NetworkBehaviour
{
    [Header("Settings")]

    [SerializeField]
    public bool canPlay;

    [SerializeField]
    private bool blockDelete;

    [SerializeField]
    private float exitTypingStateCD;

    [SerializeField]
    private int typingCap;

    [SerializeField]
    private int commandCap;

    [Header("Commands")]

    [SerializeField]
    private List<DirectionCommand> directionCommands;

    [SerializeField]
    private CMDMovement cmdMovement;

    [SerializeField]
    private List<AttackCommand> commands;

    [Header("References")]

    [SerializeField]
    private Color32 validCommandColor;

    [SerializeField]
    private Color32 unvalidCommandColor;

    // References
    private PlayerCooldown playerCooldown;
    private TextMeshPro commandText;

    // Variables
    private List<string> currentCommand;
    [System.NonSerialized] public bool onCooldown;

    // Start is called before the first frame update
    void Start()
    {
        // Assign Command Txt
        commandText = transform.GetChild(1).GetComponent<TextMeshPro>();

        // Assign cooldown script
        playerCooldown = GetComponent<PlayerCooldown>();

        // Assign scripts to their commands
        CMDAttack[] atkScripts = transform.GetChild(0).GetComponents<CMDAttack>();
        foreach (AttackCommand atkCmd in commands) 
        {
            foreach (CMDAttack atkScript in atkScripts) 
            {
                if (atkScript.commandName == atkCmd.commandName)
                    atkCmd.attackScript = atkScript;
            }
        }

        currentCommand = new List<string>();
    }

    private void OnKeyPress(string key)
    {
        // Check if hit key cap
        if (currentCommand.Count >= typingCap || onCooldown) return;

        // Check if hit command cap
        if (key == "_")
        {
            int i = 0;
            foreach (string letter in currentCommand)
            {
                if (letter == "_") i++;
            }
            if (i >= commandCap - 1) return;
        }

        // Add key to command
        currentCommand.Add(key);

        // Update command text for player
        UpdateCommandText();

        // Update typing state
        ToggleTypingState();
    }

    private void OnCommandEnter()
    {
        // Check for valid commands
        List<DirectionCommand> directionCommand = GetValidDirectionCommand();
        AttackCommand command = GetValidCommand();

        // Did valid command, specifically a direction only command
        if (directionCommand != null && CheckValidCommand() == 0)
        {
            // Do command
            cmdMovement.DoCommand(directionCommand, 0);
        }
        // Did valid command
        else if (command != null)
        {
            // Do command
            foreach (AttackCommand cmd in commands) 
            {
                if (cmd.commandName == command.commandName) 
                {
                    cmd.attackScript.DoCommand(directionCommand);
                }
            }
        }
        // Did unvalid command
        else
        {
            // Apply cooldown
            playerCooldown.ApplyCooldown(exitTypingStateCD);
        }

        // Clear command text
        currentCommand.Clear();
        UpdateCommandText();

        // Update typing state
        ToggleTypingState();
    }

    // Delete command, up till a space or nothing
    private void OnCommandDelete()
    {
        if (blockDelete)
        {
            // Delete by block
            for (int i = currentCommand.Count - 1; i >= 0; i--)
            {
                if (currentCommand[currentCommand.Count - 1] == "_")
                {
                    currentCommand.RemoveAt(currentCommand.Count - 1);
                    break;
                }

                currentCommand.RemoveAt(currentCommand.Count - 1);
            }
        }
        else
        {
            // Delete normally
            if (currentCommand.Count != 0)
                currentCommand.RemoveAt(currentCommand.Count - 1);
        }

        // Apply cooldown on exit of typing state
        if (currentCommand.Count == 0)
            playerCooldown.ApplyCooldown(exitTypingStateCD);

        // Update text
        UpdateCommandText();

        // Update typing state
        ToggleTypingState();
    }

    // Clear command
    public void ClearCommand() 
    {
        // Clear command text
        currentCommand.Clear();
        UpdateCommandText();

        // Update typing state
        ToggleTypingState();
    }

    // Get current command if valid
    private List<DirectionCommand> GetValidDirectionCommand() 
    {
        // Check if first command in current command list is a valid direction
        List<string> commandList = GetCommands();

        List<DirectionCommand> dirCommands = new List<DirectionCommand>();

        foreach (string cmd in commandList) 
        {
            foreach (DirectionCommand directionalCommand in directionCommands)
            {
                if (cmd == directionalCommand.command) 
                { 
                    dirCommands.Add(directionalCommand);
                    break;
                }
            }
        }

        if (dirCommands.Count != 0)
            return dirCommands;

        return null;
    }

    // Check if current command is valid
    // 1: Valid command
    // 0: No Command Found
    // -1: Invalid command typed
    private int CheckValidCommand()
    {
        // Check if there is a valid command
        List<string> commandList = GetCommands();
        List<DirectionCommand> dirCommand = GetValidDirectionCommand();

        // Remove directional commands from command list
        for (int i = commandList.Count-1; i >= 0; i--) 
        {
            foreach (DirectionCommand directionalCommand in directionCommands) 
            {
                if (commandList[i] == directionalCommand.command) 
                {
                    commandList.RemoveAt(i);
                    break;
                }
            }
        }

        // Check if command is valid
        foreach (string cmd in commandList) 
        {
            foreach (AttackCommand command in commands)
            {
                if (cmd == command.command) 
                {
                    // Check if direction is needed
                    if (((command.directionType == AttackCommand.DirectionType.MustInclude ||
                        command.directionType == AttackCommand.DirectionType.Optional) && dirCommand != null) ||
                        ((command.directionType == AttackCommand.DirectionType.MustNotInclude ||
                        command.directionType == AttackCommand.DirectionType.Optional) && dirCommand == null))
                    {
                        return 1;
                    }
                }
            }
        }

        if (commandList.Count != 0)
            return -1;

        return 0;
    }

    // Get current command if valid
    private AttackCommand GetValidCommand()
    {
        // Check if there is a valid command
        List<string> commandList = GetCommands();

        foreach (string cmd in commandList)
        {
            foreach (AttackCommand command in commands)
            {
                if (cmd == command.command)
                {
                    // Check if valid direction state
                    if (((command.directionType == AttackCommand.DirectionType.MustInclude || command.directionType == AttackCommand.DirectionType.Optional) &&
                        GetValidDirectionCommand() != null) ||
                        ((command.directionType == AttackCommand.DirectionType.MustNotInclude || command.directionType == AttackCommand.DirectionType.Optional) &&
                        commandList.Count == 1)) 
                    {
                        return command;
                    }
                }
            }
        }

        return null;
    }

    // Return current typed commands
    private List<string> GetCommands() 
    {
        // Go thorugh current command, seperating commands by spaces
        List<string> commandList = new List<string>();

        string command = "";
        foreach (string letter in currentCommand)
        {
            if (letter == "_")
            {
                commandList.Add(command);
                command = "";
            }
            else
                command += letter;
        }
        commandList.Add(command);

        return commandList;
    }

    // Update command text for player
    private void UpdateCommandText() 
    {
        // Create string with command
        string command = "";
        foreach (string letter in currentCommand)
        {
            command += letter;
        }

        commandText.text = command;

        // Check if command is valid, changing text color
        if (CheckValidCommand() == 1 || (GetValidDirectionCommand() != null && CheckValidCommand() == 0))
            commandText.color = validCommandColor;
        else
            commandText.color = unvalidCommandColor;
    }

    // Enter a non moving state when the player is typing
    private void ToggleTypingState() 
    {
        if (currentCommand.Count > 0)
        {
            // Enter typing state
            cmdMovement.FreezeMovement(true);
        }
        else
        {
            // Exit typing state
            cmdMovement.FreezeMovement(false);
        }
    }

    // Get key input
    [Client]
    void OnGUI()
    {
        if (!hasAuthority || !canPlay) return;

        Event e = Event.current;

        //Check the type of the current event, making sure to take in only the KeyDown of the keystroke.
        //char.IsLetter to filter out all other KeyCodes besides alphabetical.
        if (e.type == EventType.KeyDown && e.keyCode.ToString().Length == 1 && char.IsLetter(e.keyCode.ToString()[0]))
        {
            //This is your desired action
            NetworkValidateKeyEvent("Key Press", e.keyCode.ToString().ToUpper());
        }

        // Allow to input space
        else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Space) { NetworkValidateKeyEvent("Space", ""); }

        // Delete command
        else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Backspace) { NetworkValidateKeyEvent("Delete", ""); }

        // Commit to command
        else if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Return) { NetworkValidateKeyEvent("Enter", ""); }

    }

    [Command]
    private void NetworkValidateKeyEvent(string eventType, string key) 
    {
        NetworkKeyEvent(eventType, key);
    }

    [ClientRpc]
    private void NetworkKeyEvent(string eventType, string key) 
    {
        if (eventType == "Key Press")
        {
            OnKeyPress(key);
        }

        else if (eventType == "Space")
        {
            OnKeyPress("_");
        }

        else if (eventType == "Delete")
        {
            OnCommandDelete();
        }

        else if (eventType == "Enter")
        {
            OnCommandEnter();
        }
    }
}
