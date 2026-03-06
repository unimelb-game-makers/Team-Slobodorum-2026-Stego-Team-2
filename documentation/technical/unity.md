# Unity Best Practices and Guidelines

### Table of Contents
- [Project Setup](#project-setup)
  - [Download Unity Hub](#download-unity-hub)
  - [Install Latest Unity LTS Version](#install-latest-unity-lts-version)
  - [Configure with GitHub](#configure-with-github)
- [Folder Structure](#folder-structure)
- [Coding Standards](#coding-standards)
  - [Pascal Case](#pascal-case)
  - [Variable Naming](#variable-naming)
  - [Function Order](#function-order)
  - [Comments](#comments)

## Project Setup

### Download Unity Hub
**What is Unity Hub?**

Unity Hub is a standalone management tool that allows you to manage multiple Unity projects and installations of the Unity Editor.

**Why use Unity Hub?**

Using Unity Hub makes it easy to manage different versions of Unity, switch between projects, and organize your development environment efficiently.

**Steps to Download Unity Hub:**
1. Go to the [Unity Hub download page](https://unity3d.com/get-unity/download).
2. Click on the “Download Unity Hub” button.
3. Install the downloaded Unity Hub installer on your computer.

### Install Latest Unity LTS Version

**What is LTS?**

LTS stands for Long Term Support. Unity LTS versions provide a stable foundation for projects, with bug fixes and improvements without introducing new features that might disrupt development.

**Why use LTS?**

Using the latest Unity LTS version ensures that your project benefits from the latest stability and security updates without the risk of new, potentially unstable features.

**Steps to Install Unity LTS Version:**
1. Open Unity Hub.
2. Go to the “Installs” tab.
3. Click on “Add” and select the latest LTS version.
4. Follow the prompts to install the selected Unity version.
5. Install any required modules (WebGL, iOS, Android, etc)

### Configure with GitHub

**Why use GitHub?**

GitHub provides version control for your project, allowing you to track changes, collaborate with team members, and revert to previous states if necessary.

**Steps to Configure Unity Project with GitHub Desktop:**

1. Clone a Repository
    - Open GitHub Desktop.
    - Click on "File" > "Clone Repository..."
    - In the "URL" tab, enter the URL of the repository you want to clone.
    - Choose the local path where you want to clone the repository.
    - Click "Clone".

2. Commit Your Changes
    - Make changes to your Unity project.
    - Open GitHub Desktop, and you should see your project files listed in the "Changes" tab.
    - Write a commit message describing the changes.
    - Click the "Commit to main" button.

3. Push Your Changes
    - After committing your changes, click the "Push origin" button at the top of GitHub Desktop to sync your changes with the GitHub repository.

4. Sync Changes
    - If there are changes in the remote repository that you don't have locally, click the "Fetch origin" button in GitHub Desktop.
    - Review the changes and, if everything looks good, click the "Pull origin" button to sync the changes to your local repository.


## Folder Structure

### Why is Folder Structure Important?
A well-organised folder structure helps keep your project manageable and makes it easier to locate and manage assets and scripts.

### Recommended Folder Structure:
- **Assets/**: Main folder for all your assets.
- **Scripts/**: All C# scripts.
- **Prefabs/**: Prefabricated objects.
- **Scenes/**: All scene files.
- **Materials/**: Material files.
- **Textures/**: Texture files.
- **Audio/**: Audio files.
- **Animations/**: Animation files and controllers.
- **UI/**: User Interface elements and related scripts.

### Example:
```plaintext
Assets/
  Scripts/
    Player/
    Enemies/
  Prefabs/
    Characters/
    Environment/
  Scenes/
    MainMenu.unity
    Level1.unity
  Materials/
    PlayerMaterials/
    EnemyMaterials/
  Textures/
    UI/
    Environment/
  Audio/
    Music/
    SFX/
  Animations/
    Player/
    Enemies/
  UI/
    MainMenu/
    InGameUI/
```

## Coding Standards
### Pascal Case
**What is Pascal Case?**

Pascal Case is a naming convention where the first letter of each word is capitalized without any spaces or underscores.

**Where to use Pascal Case?**

Use Pascal Case for naming classes, methods, and properties. E.g.
- Class: `PlayerController`
- Method: `GetPlayerHealth`
- Property: `IsAlive`

### Variable Naming
**Descriptive Names**

Use clear, descriptive names for variables that convey their purpose
- Bad: `int hp;`
- Good: `int healthPoints`

**Camel Case**

Use camel case for variable names, where the first letter is lowercase, and the first letter of each subsequent word is capitalised. E.g.
- `playerHealth`
- `enemySpeed`
- `isGameOver`

**Boolean Variables**

Start boolean variable namse with "is" or "has" to indicate a true/false value. E.g,
- `isJumping`
- `hasPowerUp`

### Function Order
**Organise Methods Logically**
Arrange methods in a logical order, typically grouping related functions together
- **Initialization Methods**: Constructor, `Awake()`, `Start()`
- **Update Methods**: `Update()`, `FixedUpdate()`, `LateUpdate()`
- **Event Handlers**: Input handlers, collision handlers
- **Utility Methods**: Helper functions, private methods

**Example**

```c#
public class PlayerController : MonoBehaviour
{
    // Initialization methods
    void Awake() { }
    void Start() { }

    // Update methods
    void Update() { }
    void FixedUpdate() { }

    // Event handlers
    void OnCollisionEnter(Collision collision) { }

    // Utility methods
    private void MovePlayer() { }
}
```

### Comments
**Why Comment?**

Comments make your code easier to understand for yourself and others, explaining the purpose of complex code sections.


**Types of Comments**

- **Single-Line Comments**: Use // for short explanations or notes.
- **Multi-Line Comments**: Use /* */ for longer explanations or block comments.
- **XML Documentation Comments**: Use /// for method or class documentation, which can be used by IDEs to provide additional information.

**Example**

This example is a bit overkill. You don't have to comment everything, but this is an example of how to use every different type of comment in one method.
```c#
/// <summary>
/// Calculates the player's score.
/// </summary>
/// <param name="points">The points to add to the score.</param>
void CalculateScore(int points)
{
    // Initial score value, starting at zero.
    int score = 0;

    /*
     * Check if the points parameter is valid.
     * If points is less than zero, log a warning and return without changing the score.
     * This prevents the score from being inadvertently decreased.
     */
    if (points < 0)
    {
        Console.WriteLine("Warning: Points cannot be negative.");
        return;
    }

    // Add the points to the score.
    score += points;
}
```
