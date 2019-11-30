using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class LimitedDepthSearch : SearchAlgorithm {

	//we use a stack instead of a queue
	private Stack<SearchNode> openStack = new Stack<SearchNode> ();
	private HashSet<object> closedSet = new HashSet<object> ();

	//this is going to be the limit
	public int max = 0;

	void Start ()
	{
		problem = GameObject.Find ("Map").GetComponent<Map> ().GetProblem();
		SearchNode start = new SearchNode (problem.GetStartState (), 0);
		openStack.Push(start);
	}

	//in this method we aplly the depth first search algorithm with a cost limit
	protected override void Step()
	{
		if (openStack.Count > 0)
		{
			SearchNode cur_node = openStack.Pop();
			closedSet.Add (cur_node.state);

			if (problem.IsGoal (cur_node.state)) {
				solution = cur_node;
				finished = true;
				running = false;
			} else {
				Successor[] sucessors = problem.GetSuccessors (cur_node.state);
				foreach (Successor suc in sucessors) {
					SearchNode new_node = new SearchNode (suc.state, suc.cost + cur_node.g, suc.action, cur_node);
					//if the closed set doesn't contain the successor and it's cost is less than the max we push it to the stack
					if (!closedSet.Contains (suc.state) && new_node.f <= max) {
						openStack.Push (new_node);
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

}
