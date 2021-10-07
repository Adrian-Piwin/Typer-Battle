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
}

[System.Serializable]
public class Command
{
    // Name of command 
    public string commandName;
    // Command to type to go that direction
    public string command;

    // Whether a command relies on a direction command or not to work
    public enum DirectionType 
    {
        MustInclude,
        MustNotInclude,
        Optional
    };

    public DirectionType direction;

}

public class PlayerCommands : MonoBehaviour
{
    [Header("Movement Commands")]

    [SerializeField]
    private List<DirectionCommand> directionCommands;

    [SerializeField]
    private List<Command> commands;

    [Header("References")]

    [SerializeField]
    private TextMeshPro commandText;

    private List<string> currentCommand;

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
        currentCommand.Add(key);

        // Update command text for player
        UpdateCommandText();
    }

    private void OnCommandEnter() 
    {
        List<string> commandList = GetCommands();

        // Do command
        foreach (string command in commandList) {
            Debug.Log(command);
        }

        // Clear command text
        currentCommand.Clear();
        UpdateCommandText();
    }

    // Delete command, up till a space or nothing
    private void OnCommandDelete() 
    {
        for (int i = currentCommand.Count-1; i >= 0; i--)
        {
            if (currentCommand[currentCommand.Count - 1] == " ")
            {
                currentCommand.RemoveAt(currentCommand.Count - 1);
                break;
            }

            currentCommand.RemoveAt(currentCommand.Count-1);
        }
        
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

    // Return current typed commands
    private List<string> GetCommands() 
    {
        // Go thorugh current command, seperating commands by spaces
        List<string> commandList = new List<string>();

        string command = "";
        foreach (string letter in currentCommand)
        {
            if (letter == " ")
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
        if (CheckValidDirectionCommand() != null)
            commandText.color = new Color32(0, 255, 0, 150);
        else
            commandText.color = new Color32(0, 0, 0, 150);
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
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Space) { OnKeyPress(" "); }

        // Delete command
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Backspace) { OnCommandDelete(); }

        // Commit to command
        if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Return) { OnCommandEnter(); }

    }
}
