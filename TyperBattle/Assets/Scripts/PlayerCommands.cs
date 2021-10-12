using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

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
public class Command
{
    // Name of command 
    public string commandName;
    // Command to type to go that direction
    public string command;
    // Cooldown after command
    public float cooldown;

    // Whether a command relies on a direction command or not to work
    public enum DirectionType 
    {
        MustInclude,
        MustNotInclude,
        Optional
    };

    // Whether a command relies on to be grounded or not to work
    public enum GroundedType
    {
        MustBeGrounded,
        MustNotBeGrounded,
        Optional
    };

    public DirectionType directionType;
    public GroundedType groundedType;

}

public class PlayerCommands : MonoBehaviour
{
    [Header("Settings")]

    [SerializeField]
    private bool isPlaying;

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
    private List<Command> commands;

    [Header("References")]

    [SerializeField]
    private PlayerManager playerManager;

    [SerializeField]
    private TextMeshPro commandText;

    [SerializeField]
    private PlayerCooldown playerCooldown;

    [SerializeField]
    private Color32 validCommandColor;

    [SerializeField]
    private Color32 unvalidCommandColor;

    [Header("Command References")]

    [SerializeField]
    private CMDMovement cmdMovement;

    [SerializeField]
    private CMDLightAtk cmdLightAtk;

    [SerializeField]
    private CMDHeavyAtk cmdHeavyAtk;

    [SerializeField]
    private CMDSlideAtk cmdSlideAtk;

    private List<string> currentCommand;

    [System.NonSerialized]
    public bool onCooldown;

    // Start is called before the first frame update
    void Start()
    {
        currentCommand = new List<string>();
    }

    private void OnKeyPress(string key)
    {
        // Check if hit key cap
        if (currentCommand.Count >= typingCap) return;

        // Check if hit command cap
        if (key == "_") 
        {
            int i = 0;
            foreach (string letter in currentCommand)
            {
                if (letter == "_") i++;
            }
            if (i >=  commandCap-1) return;
        }

        // Add key to command
        if (!onCooldown)
            currentCommand.Add(key);

        // Update command text for player
        CheckSlowmoState();
        UpdateCommandText();
    }

    private void OnCommandEnter() 
    {
        // Check for valid commands
        List<DirectionCommand> directionCommand = GetValidDirectionCommand();
        Command command = GetValidCommand();
        
        // Did valid command, specifically a direction only command
        if (directionCommand != null && CheckValidCommand() == 0)
        {
            // Do command
            cmdMovement.DoCommand(directionCommand);
        }
        // Did valid command
        else if (command != null)
        {
            // Do command
            switch (command.commandName) 
            {
                case "Light":
                    cmdLightAtk.DoCommand(directionCommand, command);
                    break;

                case "Heavy":
                    cmdHeavyAtk.DoCommand(directionCommand, command);
                    break;

                case "Slide":
                    cmdSlideAtk.DoCommand(directionCommand, command);
                    break;
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
        CheckSlowmoState();
        UpdateCommandText();
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

        CheckSlowmoState();
        UpdateCommandText();
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
            foreach (Command command in commands)
            {
                if (cmd == command.command) 
                {
                    // Check if must include direction and direction is included
                    if ((command.directionType == Command.DirectionType.MustInclude ||
                        command.directionType == Command.DirectionType.Optional) && dirCommand != null)
                    {
                        // Check if valid grounded state
                        if (((command.groundedType == Command.GroundedType.MustBeGrounded ||
                            command.groundedType == Command.GroundedType.Optional) && playerManager.isGrounded()) ||
                            ((command.groundedType == Command.GroundedType.MustNotBeGrounded ||
                            command.groundedType == Command.GroundedType.Optional) && !playerManager.isGrounded()))
                        {
                            return 1;
                        }
                    }

                    // Check if must not include direction and direction is not included
                    else if ((command.directionType == Command.DirectionType.MustNotInclude ||
                        command.directionType == Command.DirectionType.Optional) && dirCommand == null) 
                    {
                        // Check if valid grounded state
                        if (((command.groundedType == Command.GroundedType.MustBeGrounded ||
                            command.groundedType == Command.GroundedType.Optional) && playerManager.isGrounded()) ||
                            ((command.groundedType == Command.GroundedType.MustNotBeGrounded ||
                            command.groundedType == Command.GroundedType.Optional) && !playerManager.isGrounded()))
                        {
                            return 1;
                        }
                    }
                }
            }
        }

        if (commandList.Count != 0)
            return -1;

        return 0;
    }

    // Get current command if valid
    private Command GetValidCommand()
    {
        // Check if there is a valid command
        List<string> commandList = GetCommands();

        foreach (string cmd in commandList)
        {
            foreach (Command command in commands)
            {
                if (cmd == command.command)
                {
                    // Check if valid direction state
                    if (((command.directionType == Command.DirectionType.MustInclude || command.directionType == Command.DirectionType.Optional) &&
                        GetValidDirectionCommand() != null) ||
                        ((command.directionType == Command.DirectionType.MustNotInclude || command.directionType == Command.DirectionType.Optional) &&
                        commandList.Count == 1)) 
                    {
                        // Check if valid grounded state
                        if (((command.groundedType == Command.GroundedType.MustBeGrounded ||
                            command.groundedType == Command.GroundedType.Optional) && playerManager.isGrounded()) ||
                            ((command.groundedType == Command.GroundedType.MustNotBeGrounded ||
                            command.groundedType == Command.GroundedType.Optional) && !playerManager.isGrounded())) 
                        {
                            return command;
                        }
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

    // Check to enter slowmo state
    private void CheckSlowmoState()
    {
        if (currentCommand.Count != 0)
            cmdMovement.EnterSlowmo();
        else
            cmdMovement.ExitSlowmo();
    }

    // Get key input
    void OnGUI()
    {
        if (!isPlaying) return;

        Event e = Event.current;

        //Check the type of the current event, making sure to take in only the KeyDown of the keystroke.
        //char.IsLetter to filter out all other KeyCodes besides alphabetical.
        if (e.type == EventType.KeyDown && e.keyCode.ToString().Length == 1 && char.IsLetter(e.keyCode.ToString()[0]))
        {
            //This is your desired action
            OnKeyPress(e.keyCode.ToString().ToUpper());
        }

        // Allow to input space
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Space) { OnKeyPress("_"); }

        // Delete command
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Backspace) { OnCommandDelete(); }

        // Commit to command
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Return) { OnCommandEnter(); }

    }
}
