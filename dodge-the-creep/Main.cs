using Godot;
using System;
using System.ComponentModel;

public partial class Main : Node
{
	// Called when the node enters the scene tree for the first time.
	[Export]
	public PackedScene MobScene { get; set; }

	private int _score;

	public void GameOver()
	{
		GetNode<Timer>("MobTimer").Stop();
		GetNode<Timer>("ScoreTimer").Stop();

		GetNode<AudioStreamPlayer>("Music").Stop();
		GetNode<AudioStreamPlayer>("DeathSound").Play();
		GetNode<Hud>("HUD").ShowGameOver();
		
	}

	public void NewGame()
	{
		GetTree().CallGroup("mobs", Node.MethodName.QueueFree);
		_score = 0;
		var player = GetNode<Player>("Player");
		var startPosition = GetNode<Marker2D>("StartPosition");
		player.Start(startPosition.Position);

		GetNode<Timer>("StartTimer").Start();

		var hud = GetNode<Hud>("HUD");
		hud.UpdateScore(_score);
		hud.ShowMessage("Get Ready!");
		GetNode<AudioStreamPlayer>("Music").Play();

	}

	private void OnScoreTimerTimeOut()
	{
		_score++;
		GetNode<Hud>("HUD").UpdateScore(_score);
	}

	private void onStartTimerTimeOut()
	{
		GetNode<Timer>("MobTimer").Start();
		GetNode<Timer>("ScoreTimer").Start();
	}

	private void OnMobTimerTimeout()
	{
		//create new instance of Mob Scene
		Mob mob = MobScene.Instantiate<Mob>();

		//choose random location on Path2D
		var mobSpawnLocation = GetNode<PathFollow2D>("MobPath/MobSpawnLocation");
		mobSpawnLocation.ProgressRatio = GD.Randf();

		//set mob's direction perpendicular to the path direction
		float direction = mobSpawnLocation.Rotation + Mathf.Pi / 2;

		//add some randomness to the directions
		direction += (float) GD.RandRange(-Mathf.Pi / 4, Math.PI / 4);
		mob.Rotation = direction;

		//set the mob position to a random location
		mob.Position = mobSpawnLocation.Position;

		//choose velocity
		var velocity = new Vector2((float) GD.RandRange(150.0, 250.0), 0);
		mob.LinearVelocity = velocity.Rotated(direction);

		//spawn the mob by add it to the main scene
		AddChild(mob);

	}

	public override void _Ready()
	{
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
