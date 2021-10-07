using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

// Command classes
[System.Serializable]
public class DirectionCommand
{
    // Direction to move
    public string direction;
    // Command to type to go that direction
    public string command;
    // Cooldown after command
    public float cooldown;
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

    public DirectionType directionType;

}

public class PlayerCommands : MonoBehaviour
{
    [Header("Settings")]

    [SerializeField]
    private bool blockDelete;

    [SerializeField]
    private float exitTypingStateCD;

    [Header("Commands")]

    [SerializeField]
    private List<DirectionCommand> directionCommands;

    [SerializeField]
    private List<Command> commands;

    [Header("References")]

    [SerializeField]
    private TextMeshPro commandText;

    [SerializeField]
    private PlayerSlowmo playerSlowmo;

    [SerializeField]
    private PlayerCooldown playerCooldown;

    [SerializeField]
    private Color32 validCommandColor;

    [SerializeField]
    private Color32 unvalidCommandColor;

    [Header("Command References")]

    [SerializeField]
    private CMDMovement cmdMovement;

    private List<string> currentCommand;

    [System.NonSerialized]
    public bool onCooldown;

    // Start is called before the first frame update
    void Start()
    {
        currentCommand = new List<string>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnKeyPress(string key)
    { 
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
        DirectionCommand directionCommand = CheckValidDirectionCommand();
        Command command = CheckValidCommand();
        
        // Did valid command, specifically a direction only command
        if (directionCommand != null && GetCommands().Count == 1)
        {
            // Do command
            cmdMovement.DoCommand(directionCommand);

            // Apply cooldown
            playerCooldown.ApplyCooldown(directionCommand.cooldown);
        }
        // Did valid command
        else if (command != null)
        {
            // Do command

            // Apply cooldown
            playerCooldown.ApplyCooldown(command.cooldown);
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
            currentCommand.RemoveAt(currentCommand.Count - 1);
        }

        CheckSlowmoState();
        UpdateCommandText();
    }

    // Check if current command is valid
    private DirectionCommand CheckValidDirectionCommand() 
    {
        // Check if first command in current command list is a valid direction
        List<string> commandList = GetCommands();

        foreach (DirectionCommand directionalCommand in directionCommands) 
        {
            if (commandList[0] == directionalCommand.command) { return directionalCommand; }
        }

        return null;
    }

    // Check if current command is valid
    private Command CheckValidCommand()
    {
        // Check if there is a valid command
        List<string> commandList = GetCommands();

        foreach (string command in commandList) 
        {
            foreach (Command cmd in commands)
            {
                if (command == cmd.command) 
                {
                    if (((cmd.directionType == Command.DirectionType.MustInclude || cmd.directionType == Command.DirectionType.Optional) &&
                        CheckValidDirectionCommand() != null) ||
                        ((cmd.directionType == Command.DirectionType.MustNotInclude || cmd.directionType == Command.DirectionType.Optional) &&
                        commandList.Count == 1))
                        return cmd;
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
        if (CheckValidCommand() != null || (CheckValidDirectionCommand() != null && GetCommands().Count == 1))
            commandText.color = validCommandColor;
        else
            commandText.color = unvalidCommandColor;
    }

    // Check to enter slowmo state
    private void CheckSlowmoState()
    {
        if (currentCommand.Count != 0)
            playerSlowmo.EnterSlowmo();
        else
            playerSlowmo.ExitSlowmo();
    }

    // Get key input
    void OnGUI()
    {

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
