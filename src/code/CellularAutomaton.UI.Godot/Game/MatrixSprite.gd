extends Sprite

func _input(event):
	if event.is_action_pressed("zoom_in"):
		scale *0.9;
	if event.is_action_pressed("zoom_out"):
		scale *= 1.1;
