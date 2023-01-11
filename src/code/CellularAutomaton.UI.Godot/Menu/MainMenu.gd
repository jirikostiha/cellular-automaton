extends Control

onready var start_btn := $"%Start"

func _ready():
	start_btn.grab_focus()

func _on_Start_pressed():
	SceneSwitcher.change_scene(SceneSwitcher.scene.GAME)

func _on_Quit_pressed():
	SceneSwitcher.quit()
