extends Resource

class_name PuzzleData

export var Id: int = 0
export var FrameDimensions: Vector2 = Vector2(0, 0)
export var Scramble: Array = []
export var NullIds: Array = []


func init(id: int, frameDimensions: Vector2, scramble: Array, nullIds: Array) -> void:
	Id = id
	FrameDimensions = frameDimensions
	Scramble = scramble
	NullIds = nullIds
