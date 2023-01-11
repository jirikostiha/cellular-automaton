# SceneSwitcher.gd
extends Node

signal scene_changed(old, new)

enum scene { GAME, MAIN_MENU }

var _scene_map = { 
	scene.GAME       : "res://Game/Game.tscn",
	scene.MAIN_MENU  : "res://Menu/MainMenu.tscn"}
	
var scene_params = null

func _ready():
	pass

func change_scene(next_scene: int, params=null):
	change_scene_by_path(_scene_map[next_scene], params)

# Call this instead to be able to provide arguments to the next scene
func change_scene_by_path(next_scene: String, params=null):
	scene_params = params
	assert(get_tree().change_scene(next_scene) == OK);
	#var error_code = get_tree().change_scene(next_scene);
	#if error_code != OK:
		#print("ERROR: Unavailable scene", error_code)
	var current_scene_name = get_tree().get_current_scene().get_name()	
	emit_signal("scene_changed", current_scene_name, next_scene)
	if get_tree().paused:
		print("unpausing tree")
		get_tree().paused = false

func change_scene_to(next_scene: PackedScene, params=null):
	scene_params = params
	assert(get_tree().change_scene_to(next_scene) == OK);
	var current_scene_name = get_tree().get_current_scene().get_name()
	emit_signal("scene_changed", current_scene_name, next_scene.resource_name)
	if get_tree().paused:
		print("unpausing tree")
		get_tree().paused = false

func append_scene(next_scene: int, params=null):
	append_scene_by_path(_scene_map[next_scene], params)
	
func append_scene_by_path(next_scene: String, params=null):
	scene_params = params
	var scene = load(next_scene).instance()
	get_tree().current_scene.add_child(scene)


func show_scene(scene, pause_tree=false):
	scene.visible = true
	if pause_tree:
		print("pausing tree")
		get_tree().paused = true

func hide_scene(scene, unpause_tree=false):
	scene.visible = false
	if unpause_tree:
		print("unpausing tree")
		get_tree().paused = false

# In the newly opened scene, you can get the parameters by name
func get_param(name):
	if scene_params != null and scene_params.has(name):
		return scene_params[name]
	return null

func quit():
	get_tree().quit()
	
#Usage
# In the calling scene
#SceneSwitcher.change_scene("res://Game/Game.tscn", {"location":selected_location})

# In the new scene
#var current_location = SceneSwitcher.get_param("location")
