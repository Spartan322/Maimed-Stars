extends Resource

export(String) var Name : String
export(float) var MaxSpeed: float = 100
export(float) var MaxAcceleration: float = 50
export(float) var MaxAnglularSpeedDegree: float = 180
export(float) var Mass: float = 1
export(float) var BrakingRadius: float = 50
export(float) var GoalReachedThreshold: float = 0.9
export(float) var RotationAimThresholdDegrees : float = 7

func _init(name : String = "",
	maxSpeed : float = 100,
	maxAccel : float = 50,
	maxAngularSpeed : float = 180,
	mass : float = 1,
	brakingRadius : float = 50,
	goalReached : float = 0.9,
	rotationAim : float = 7): 
	Name = name;
	MaxSpeed = maxSpeed;
	MaxAcceleration = maxAccel;
	MaxAnglularSpeedDegree = maxAngularSpeed;
	Mass = mass;
	BrakingRadius = brakingRadius;
	GoalReachedThreshold = goalReached;
	RotationAimThresholdDegrees = rotationAim;
