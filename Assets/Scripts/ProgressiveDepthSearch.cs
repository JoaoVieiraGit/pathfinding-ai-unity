using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ProgressiveDepthSearch : SearchAlgorithm {

	private Stack<SearchNode> openStack = new Stack<SearchNode> ();
	private HashSet<object> closedSet = new HashSet<object> ();

	//just like limited depth search we have a max, but in this algorithm it's
	//going to be automatically incremented
	public int max = 1;

	void Start ()
	{
		problem = GameObject.Find ("Map").GetComponent<Map> ().GetProblem();
	}

	protected override void Step()
	{
		//while de depth search doesn't return success(1) it increments the max and searches again
		while (DepthSearchAlgorithm (max) != 1) {
			max++;
		}

		Debug.Log("Final Max == " + max);
	}

	public int DepthSearchAlgorithm (int max) {

		//we need to clear the stack and the closed set because everytime we reenter this method we have to start the search from the initial node
		openStack.Clear ();
		closedSet.Clear ();

		//as we have to start the search from the inital node everytime we select the inital node and add it to the stack
		SearchNode start = new SearchNode (problem.GetStartState (), 0);
		openStack.Push(start);

		while (openStack.Count > 0)
		{
			SearchNode cur_node = openStack.Pop();
			closedSet.Add (cur_node.state);

			if (problem.IsGoal (cur_node.state)) {
				solution = cur_node;
				finished = true;
				running = false;
				return 1;
			} else {
				Successor[] sucessors = problem.GetSuccessors (cur_node.state);
				foreach (Successor suc in sucessors) {
					SearchNode new_node = new SearchNode (suc.state, suc.cost + cur_node.g, suc.action, cur_node);
					//just like limitedDepthSearch
					if (!closedSet.Contains (suc.state) && new_node.f < max) {
						openStack.Push (new_node);
					}
				}
			}
		}
			finished = true;
			running = false;
			return -1;
	}
}
