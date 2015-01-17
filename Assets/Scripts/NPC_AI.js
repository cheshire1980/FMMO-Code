#pragma strict


private var npc_spawnlocation : Vector3;
private var npc_roamreach : int = 10f;

private var npc_changeflag : boolean = false;
private var directionTimer : int;
private var directionCounter : float;


function Start ()
{
	npc_spawnlocation = transform.position;
	grounded();

	directionCounter = Time.fixedTime;
	directionTimer = Random.Range(0,7);

	changeDirection();
}

function grounded ()
{
	var rcHit : RaycastHit;
	var theRay : Vector3 = transform.TransformDirection(Vector3.down);
	
	if (Physics.Raycast(transform.position, theRay, rcHit))
	{
		var GroundDis = rcHit.distance;
		transform.localPosition.y = (transform.localPosition.y - GroundDis)+1;
	}
}

function changeDirection ()
{
	var npc_roamreach_neg : int =- npc_roamreach;
	var rand_x : int = Random.Range(npc_roamreach_neg, npc_roamreach);
	var rand_y : int = Random.Range(npc_roamreach_neg, npc_roamreach);
	var rand_z : int = Random.Range(npc_roamreach_neg, npc_roamreach);
	var rand_forward : Vector3 = Vector3(rand_x, 0, rand_z);
	
	//transform.LookAt(rand_forward);
	var relativePos = rand_forward - transform.position;
	var newRot = Quaternion.LookRotation(relativePos);
	transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * 1.50);
	
	npc_changeflag = false;
}

function Update ()
{
	grounded();
	transform.Translate(0, 0, Time.deltaTime * 1.20);
	
	var npc_location : Vector3 = transform.position;
	
	if (Vector3.Distance(npc_location, npc_spawnlocation) > npc_roamreach)
	{
		var relativePos = npc_spawnlocation - transform.position;
		var newRot = Quaternion.LookRotation(relativePos);
		transform.rotation = Quaternion.Lerp(transform.rotation, newRot, Time.deltaTime * 1.50);
	}
	
	if (Time.fixedTime - directionCounter >= directionTimer)
	{
		changeDirection();
		directionCounter = Time.fixedTime;
		directionTimer = Random.Range(0,7);
	}
}