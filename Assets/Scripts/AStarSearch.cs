using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class AStarSearch : SearchAlgorithm {

	//we used to have the nodes in a List that we sorted but instead chose to use an implementation of priority queue
	//private List<SearchNode> openList = new List<SearchNode>();

	//hashMap that contains the positions of corners in the map, simple deadlocks
	private HashSet<Vector2> simpleDeadlocksHash = new HashSet<Vector2>();

	//priority queue implementation used to store the nodes
	private PriorityQueue<float, SearchNode> openQueue = new PriorityQueue<float, SearchNode>();
	private HashSet<object> closedSet = new HashSet<object> ();
	private Map map;

	void Start ()
	{
		map = GameObject.Find ("Map").GetComponent<Map> ();
		problem = map.GetProblem();

		SearchNode start = new SearchNode (problem.GetStartState (), 0);
		//openList.Add(start);

		//Add first node to priority queue, they are sorted by f (g+h)
		openQueue.Enqueue(start, start.f);
	}

	protected override void Step()
	{
		float tempHeuristics;

		//if (openList.Count > 0)

		if (!openQueue.IsEmpty)
		{
			//openList = openList.OrderBy (node => node.f).ToList();
			//SearchNode cur_node = (SearchNode)openList [0];

			SearchNode cur_node = (SearchNode) openQueue.Dequeue();
			closedSet.Add (cur_node.state);

			//openList.RemoveAt (0);

			if (problem.IsGoal (cur_node.state)) {
				solution = cur_node;
				finished = true;
				running = false;
			} else {
				Successor[] sucessors = problem.GetSuccessors (cur_node.state);
				foreach (Successor suc in sucessors) {

					//for each successor determine heuristic
					tempHeuristics = GetHeuristicsD (suc.state);
					//create the node
					SearchNode new_node = new SearchNode (suc.state, suc.cost + cur_node.g, tempHeuristics, suc.action, cur_node);

					//if the node isn't in the closed set and there aren't any crates in deadlock state we add it to the queue
					if ((!closedSet.Contains (suc.state))) {
						//openList.Add(new_node);

						openQueue.Enqueue (new_node, new_node.f);
					}
				}
			}
		}
		else
		{
			finished = true;
			running = false;
		}
	}
	


	//Initial heuristics where we return the number of remaining goals
	public float GetHeuristics (object state)
	{
		List<Vector2> goals;
		goals = map.GetGoals ();

		SokobanState s = (SokobanState)state;
		int remainingGoals = goals.Count;


		foreach (Vector2 crate in s.crates) {
			if (goals.Contains (crate)) {
				remainingGoals--;
			}
		}

		return (float)remainingGoals;
	}

	//simple heuristics where we sum the distance of all the boxes to their respective closest goal
	public float GetHeuristicsB(object state) {
		List<Vector2> goals;
		goals = map.GetGoals ();
		SokobanState s = (SokobanState)state;

		float score = 0;
		float minDistance;
		float tempDistance;

		//for each box on the floor, calculate the distance to each goal
		foreach (Vector2 crate in s.crates) {
			minDistance = 1000;
			foreach (Vector2 goal in goals) {
				tempDistance = Vector2.Distance (crate, goal);
				if (tempDistance < minDistance) {
					minDistance = tempDistance;
				}
			}
			score += minDistance;
		}

		return score;
	}

	//Heuristics function uses Manhattan distance between player and
	//nearest box, and between the boxes and the goals.
	public float GetHeuristicsC (object state) {
		List<Vector2> goals;
		goals = map.GetGoals ();
		SokobanState s = (SokobanState)state;

		Vector2 player = s.player;

		//the lower the better
		float score = 0;

		float playerdist = 1000;


		//for each box on the floor, calculate the distance to each empty goal
		foreach (Vector2 crate in s.crates) {
			foreach (Vector2 goal in goals) {
				//calculate distance of x and y cords
				float xdist = crate.x - goal.x;
				float ydist = crate.y - goal.y;

				//take absolute value of distance
				if (xdist < 0)
					xdist *= -1;
				if (ydist < 0)
					ydist *= -1;

				//add distance to score, lower score = better
				score += xdist;
				score += ydist;
			}

			//calculate player to box distances
			float p_to_box_x = crate.x - player.x;
			float p_to_box_y = crate.y - player.y;

			//take absolute value of distance
			if (p_to_box_x < 0)
				p_to_box_x *= -1;
			if (p_to_box_y < 0)
				p_to_box_y *= -1;

			//stores shortest distance to any box
			if (playerdist > (p_to_box_y + p_to_box_x))
				playerdist = (p_to_box_y + p_to_box_x);
		}
		score += playerdist;
		return score;
	}

	//Heuristics function uses Manhattan distance between player and
	//nearest box, and between box and nearest goal
	public float GetHeuristicsD (object state) {
		List<Vector2> goals;
		goals = map.GetGoals ();
		SokobanState s = (SokobanState)state;

		Vector2 player = s.player;

		//the lower the better
		float score = 0;

		float playerdist = 1000;
		float mindist = 1000;

		//for each box on the floor, calculate the distance to each empty goal
		foreach (Vector2 crate in s.crates) {
			mindist = 1000;
			foreach (Vector2 goal in goals) {

				float sum;
				//calculate distance of x and y cords
				float xdist = crate.x - goal.x;
				float ydist = crate.y - goal.y;

				//take absolute value of distance
				if (xdist < 0)
					xdist *= -1;
				if (ydist < 0)
					ydist *= -1;

				sum = xdist + ydist;

				if (sum < mindist)
					mindist = sum;

			}

			score += mindist;
			//calculate player to box distances
			float p_to_box_x = crate.x - player.x;
			float p_to_box_y = crate.y - player.y;

			//take absolute value of distance
			if (p_to_box_x < 0)
				p_to_box_x *= -1;
			if (p_to_box_y < 0)
				p_to_box_y *= -1;

			//stores shortest distance to any box
			if (playerdist > (p_to_box_y + p_to_box_x))
				playerdist = (p_to_box_y + p_to_box_x);
		}
		score += playerdist;
		Debug.LogWarning ("SCORE == " + score);
		return score;
	}

	public float ZeroHeuristics(object state) {
		return 0;
	}

}
