# Godot Best Practices and Guidelines

### Table of Contents
- [Project Setup](#project-setup)
  - [Download Godot Engine](#download-godot-engine)
  - [Configure with GitHub](#configure-with-github)
- [Folder Structure](#folder-structure)
- [Coding Standards](#coding-standards)
  - [Class Naming](#class-naming)
  - [Variable Naming](#variable-naming)
  - [Function Order](#function-order)
  - [Comments](#comments)

## Project Setup

### Download Godot Engine
**What is Godot Engine?**

Godot Engine is an open-source game engine that provides a comprehensive set of tools to create both 2D and 3D games.

**Why use Godot Engine?**

Godot Engine is highly flexible, lightweight, and completely free with no strings attached. It supports a wide range of platforms and has a very active community.

**Steps to Download Godot Engine:**
1. Go to the [Godot Engine download page](https://godotengine.org/download).
2. Download the latest stable version for your operating system.
3. Extract the downloaded archive and run the Godot executable.

### Configure with GitHub

**Why use GitHub?**

GitHub provides version control for your project, allowing you to track changes, collaborate with team members, and revert to previous states if necessary.

**Steps to Configure Godot Project with GitHub Desktop:**

1. **Clone a Repository:**
    - Open GitHub Desktop.
    - Click on "File" > "Clone Repository..."
    - In the "URL" tab, enter the URL of the repository you want to clone.
    - Choose the local path where you want to clone the repository.
    - Click "Clone".

2. **Commit Your Changes:**
    - Make changes to your Godot project.
    - Open GitHub Desktop, and you should see your project files listed in the "Changes" tab.
    - Write a commit message describing the changes.
    - Click the "Commit to main" button.

3. **Push Your Changes:**
    - After committing your changes, click the "Push origin" button at the top of GitHub Desktop to sync your changes with the GitHub repository.

4. **Sync Changes:**
    - If there are changes in the remote repository that you don't have locally, click the "Fetch origin" button in GitHub Desktop.
    - Review the changes and, if everything looks good, click the "Pull origin" button to sync the changes to your local repository.

## Folder Structure

### Why is Folder Structure Important?
A well-organised folder structure helps keep your project manageable and makes it easier to locate and manage assets and scripts.

### Recommended Folder Structure:
- **scripts/**: All GDScript files.
- **scenes/**: All scene files.
- **materials/**: Material files.
- **textures/**: Texture files.
- **audio/**: Audio files.
- **animations/**: Animation files and controllers.
- **ui/**: User Interface elements and related scripts.

### Example:
```plaintext
scripts/
  player/
  enemies/
scenes/
  main_menu.tscn
  level_1.tscn
materials/
  player_materials/
  enemy_materials/
textures/
  ui/
  environment/
audio/
  sfx/
  bgm/
animations/
  player/
  enemies/
ui/
  main_menu/
  game_ui/
```

## Coding Standards
### Class Naming
**Descriptive Names**

Use clear, descriptive names for classes.

**Pascal Case**

Pascal Case is a naming convention where the first letter of each word is capitalized without any spaces or underscores.
Use Pascal Case for naming clases E.g.
- `class_name PlayerController`
- `class_name Item`

### Variable Naming
**Descriptive Names**

Use clear, descriptive names for variables that convey their purpose
- Bad: `var hp`
- Good: `var health_points`

**Snake Case**

Use snake case for variable names, where everything is lowercase and words are separated with an underscore. E.g.
- `player_health`
- `enemy_speed`
- `is_game_over`

**Boolean Variables**

Start boolean variable namse with "is_" or "has_" to indicate a true/false value. E.g,
- `is_jumping`
- `has_power_fup`

### Function Order
**Organise Methods Logically**

Arrange methods in a logical order, typically grouping related functions together
- **Initialization Methods**: `_ready()`, `_enter_tree()`, `_exit_tree()`
- **Update Methods**: `_process(delta)`, `_physics_process(delta)`
- **Event Handlers**: Input handlers, collision handlers
- **Utility Methods**: Helper functions

**Example**

```gdscript
extends Node

# Initialization methods
func _ready():
    pass

func _enter_tree():
    pass

func _exit_tree():
    pass

# Update methods
func _process(delta):
    pass

func _physics_process(delta):
    pass

# Event handlers
func _on_area_entered(area):
    pass

# Utility methods
func move_player():
    pass
```

### Comments
**Why Comment?**

Comments make your code easier to understand for yourself and others, explaining the purpose of complex code sections.


**Types of Comments**

- **Single-Line Comments**: Use '#' for short explanations or notes.
- **Multi-Line Comments**: Use '#' for each line of a multi-line comment.
**Example**

This example is a bit overkill. You don't have to comment everything, but this is an example of how to use every different type of comment in one method.
```gdscript
extends Node2D
## A brief description of the class's role and functionality.
##
## The description of the script, what it can do,
## and any further detail.
##
## @tutorial:            https://the/tutorial1/url.com
## @tutorial(Tutorial2): https://the/tutorial2/url.com
## @experimental

## The description of a constant.
const GRAVITY = 9.8

## The description of a signal.
signal my_signal

## This is a description of the below enum.
enum Direction {
	## Direction up.
	UP = 0,
	## Direction down.
	DOWN = 1,
	## Direction left.
	LEFT = 2,
	## Direction right.
	RIGHT = 3,
}

## The description of the variable v1.
var v1

## This is a multiline description of the variable v2.[br]
## The type information below will be extracted for the documentation.
var v2: int

## If the member has any annotation, the annotation should
## immediately precede it.
@export
var v3 := some_func()


## As the following function is documented, even though its name starts with
## an underscore, it will appear in the help window.
func _fn(p1: int, p2: String) -> int:
	return 0


# The below function isn't documented and its name starts with an underscore
# so it will treated as private and will not be shown in the help window.
func _internal() -> void:
	pass


## Documenting an inner class.
##
## The same rules apply here. The documentation must
## immediately precede the class definition.
##
## @tutorial: https://the/tutorial/url.com
## @experimental
class Inner:

	## Inner class variable v4.
	var v4


	## Inner class function fn.
	func fn(): pass
```
