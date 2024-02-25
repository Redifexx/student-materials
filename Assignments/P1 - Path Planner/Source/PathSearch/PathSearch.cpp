#include "PathSearch.h"
namespace ufl_cap4053
{
	namespace searches
	{
		bool PathSearch::areAdjacent(const Tile* lhs, const Tile* rhs)
		{
			int lhs_r = lhs->getRow();
			int lhs_c = lhs->getColumn();
			int rhs_r = rhs->getRow();
			int rhs_c = rhs->getColumn();
			//Brute force checking :)
			if ((((lhs_r - 1) == rhs_r) || ((lhs_r + 1) == rhs_r)) && ((lhs_c) == rhs_c))
			{
				return true;
			}
			else if (((lhs_r) == rhs_r) && (((lhs_c - 1) == rhs_c) || ((lhs_c + 1) == rhs_c)))
			{
				return true;
			}
			else if (lhs_r % 2 == 0)
			{
				if (((lhs_r - 1) == rhs_r) && (((lhs_c - 1) == rhs_c) || ((lhs_c + 1) == rhs_c)))
				{
					return true;
				}
				return false;
			}
			else
			{
				if (((lhs_r + 1) == rhs_r) && (((lhs_c - 1) == rhs_c) || ((lhs_c + 1) == rhs_c)))
				{
					return true;
				}
			}
			return false;
		}

		std::vector<Tile*> PathSearch::getAdjacent(int _r, int _c)
		{
			//Brute Forcing Neighbors Yay!
			std::vector<Tile*> adjacentTiles;

			Tile* curTile = tileMap->getTile(_r - 1, _c); //Left
			if (curTile != nullptr)
			{
				adjacentTiles.push_back(curTile);
			}

			curTile = tileMap->getTile(_r + 1, _c); //Right
			if (curTile != nullptr)
			{
				adjacentTiles.push_back(curTile);
			}

			curTile = tileMap->getTile(_r, _c - 1); //Up
			if (curTile != nullptr)
			{
				adjacentTiles.push_back(curTile);
			}

			curTile = tileMap->getTile(_r, _c + 1); //Down
			if (curTile != nullptr)
			{
				adjacentTiles.push_back(curTile);
			}


			//Checks if it's Even
			if (_r % 2 == 0)
			{
				curTile = tileMap->getTile(_r - 1, _c - 1); //LeftUp
				if (curTile != nullptr)
				{
					adjacentTiles.push_back(curTile);
				}

				curTile = tileMap->getTile(_r + 1, _c - 1); //LeftDown
				if (curTile != nullptr)
				{
					adjacentTiles.push_back(curTile);
				}
			}
			else
			{
				curTile = tileMap->getTile(_r - 1, _c + 1); //RightUp
				if (curTile != nullptr)
				{
					adjacentTiles.push_back(curTile);
				}

				curTile = tileMap->getTile(_r + 1, _c + 1); //RightDown
				if (curTile != nullptr)
				{
					adjacentTiles.push_back(curTile);
				}
			}

			return adjacentTiles;
		}

		// CLASS DEFINITION GOES HERE
		PathSearch::PathSearch()
		{
			searchQueue = new PriorityQueue<TileNode>(&isGreaterThan);
		}
		PathSearch::~PathSearch()
		{
			delete searchQueue;
		}

		void PathSearch::load(TileMap* _tileMap)
		{
			auto startTime = std::chrono::high_resolution_clock::now();
			tileMap = _tileMap;
			//Loads the tiles into an unordered map that'll be treated like nodes
			for (int r = 0; r < tileMap->getRowCount(); r++)
			{
				for (int c = 0; c < tileMap->getColumnCount(); c++)
				{
					Tile* curTile = tileMap->getTile(r, c);
					if (curTile->getWeight() != 0)
					{
						TileNode* curNode = new TileNode(curTile);
						curNode->adjacentTiles = getAdjacent(r, c);
						searchGraph[curTile] = curNode;
					}
				}
			}
			auto stop = std::chrono::high_resolution_clock::now();
			auto duration = std::chrono::duration_cast<std::chrono::microseconds>(stop - startTime);
			elapsedTimes.push_back(duration.count() / 1000000.0f);
		}

		//Sets the start and goal tiles
		void PathSearch::initialize(int startRow, int startCol, int goalRow, int goalCol)
		{
			tileMap->setStartTile(startRow, startCol);
			tileMap->setGoalTile(goalRow, goalCol);
			TileNode* startNode = searchGraph[tileMap->getStartTile()];
			root = startNode;
			root->givenCost = 0;
			root->hCost = getDistance(root->tile, tileMap->getGoalTile());
			root->updateFCost();
			searchQueue->push(*startNode); //Reference
			openTiles.insert(startNode->tile);
			visitedTiles.insert(tileMap->getStartTile());
			_isDone = false;
			std::cout << "Start Tile - Row: " << startRow << ", Column:" << startCol << std::endl;
			std::cout << "X: " << tileMap->getStartTile()->getXCoordinate() << ", Y:" << tileMap->getStartTile()->getYCoordinate() << "\n" << std::endl;
		}

		//Runs the algorithm
		void PathSearch::update(long timeslice)
		{
			auto startTime = std::chrono::high_resolution_clock::now();
			auto endTime = startTime + std::chrono::milliseconds(timeslice);
			int iterations = 0;
			if (timeslice == 0)
			{
				iterations = 1; //Dummy int to stop the while
			}
			while (((std::chrono::high_resolution_clock::now() < endTime) || (iterations == 1)) && !searchQueue->empty())
			{
				Tile* curTile = searchQueue->front().tile;
				TileNode* curNode = searchGraph[curTile];
				auto itr = openTiles.find(curNode->tile);
				if (itr != openTiles.end())
				{
					openTiles.erase(itr);
				}
				searchQueue->pop();

				curNode->tile->setFill(0x7F0000FF);
				iterations++; //to break iteration

				//If Found
				if (curNode->tile == tileMap->getGoalTile())
				{
					//Sets Goal Tile to Yellow
					curNode->tile->setFill(0xFFFFFF00);
					_isDone = true;
					pathStack.push(curNode);
					solution.push_back(curNode->tile);

					//Gonna work back up to get path using unordered map of tiles and their parents
					TileNode* curParent = curNode->parent;
					
					//Progressively adds the parent of each tile into a stack
					while (curParent != nullptr)
					{
						pathStack.push(curParent);
						solution.push_back(curParent->tile); //Adds to Overall Solution
						curParent = curParent->parent;
					}
					
					//Displays the path taken by changing color
					while (!pathStack.empty())
					{
						Tile* pathTile = pathStack.top()->tile;
						pathStack.pop();
						if (pathStack.size() > 0)
						{
							pathTile->addLineTo(pathStack.top()->tile, 0xFFFF00FF);
						}
					}

					std::cout << "Goal Found!!! :)" << std::endl;
					std::cout << "\n";
					std::cout << "Goal Tile - Row: " << curNode->tile->getRow() << ", Column:" << curNode->tile->getColumn() << std::endl;
					std::cout << "X: " << curNode->tile->getXCoordinate() << ", Y:" << curNode->tile->getYCoordinate() << "" << std::endl;
					std::cout << "\n";
					std::cout << "Heuristic Cost: " << curNode->hCost << std::endl;
					std::cout << "Given Cost: " << curNode->givenCost << std::endl;
					std::cout << "Final Cost: " << curNode->fCost << std::endl;
					std::cout << "Size of Solution Path: " << solution.size() << std::endl;
					break;
				}

				//Else
				for (Tile* adjacent : curNode->adjacentTiles)
				{
					if (visitedTiles.find(adjacent) == visitedTiles.end()) //if not visited
					{
						if (adjacent->getWeight() != 0) //if not inaccessible
						{
							searchGraph[adjacent]->givenCost = curNode->givenCost + ((tileMap->getTileRadius() * 2) * (int)searchGraph[adjacent]->weight);
							searchGraph[adjacent]->hCost = getDistance(adjacent, tileMap->getGoalTile());
							searchGraph[adjacent]->updateFCost();
							searchQueue->push(*searchGraph[adjacent]);
							openTiles.insert(adjacent);
							visitedTiles.insert(adjacent);
							searchGraph[adjacent]->parent = curNode;
						}
					}
					else
					{
						if ((curNode->givenCost + ((tileMap->getTileRadius() * 2) * (int)searchGraph[adjacent]->weight)) < searchGraph[adjacent]->givenCost)
						{
							itr = openTiles.find(adjacent);
							if (itr != openTiles.end())
							{
								openTiles.erase(itr);
							}
							searchQueue->remove(*searchGraph[adjacent]);
							searchGraph[adjacent]->givenCost = curNode->givenCost + ((tileMap->getTileRadius() * 2) * (int)searchGraph[adjacent]->weight);
							searchGraph[adjacent]->updateFCost();
							searchGraph[adjacent]->parent = curNode;
							itr = openTiles.find(adjacent);
							if (itr == openTiles.end())
							{
								searchQueue->push(*searchGraph[adjacent]);
								openTiles.insert(adjacent);
							}
						}
					}
				}
			}
		}

		void PathSearch::shutdown()
		{
			//Clears the search queue
			searchQueue->clear();

			//Clears the Visited Set
			visitedTiles.clear();

			openTiles.clear();
			
			//Safety Net for Stack
			while (!pathStack.empty())
			{
				pathStack.pop();
			}

			//Clears current Path
			solution.clear();
		}

		void PathSearch::unload()
		{
			std::vector<TileNode*> choppingBlock;
			for (auto& node : searchGraph)
			{
				TileNode* curNode = node.second;
				choppingBlock.push_back(curNode);
			}
			for (TileNode* node : choppingBlock)
			{
				delete node;
			}
			searchGraph.clear();
			//delete searchQueue;
		}

		bool PathSearch::isDone() const
		{
			return _isDone;
		}

		std::vector<Tile const*> const PathSearch::getSolution() const
		{
			double totalTime = 0.0;
			for (double time : elapsedTimes)
			{
				totalTime += time;
			}
			std::cout << "load time: " << (totalTime / elapsedTimes.size()) << std::endl;
			return solution;
		}

		bool PathSearch::isGreaterThan(TileNode const& lhs, TileNode const& rhs)
		{
			//GBFS
			//return ((lhs.hCost) >= (rhs.hCost));
			//UCS
			//return ((lhs.givenCost) >= (rhs.givenCost));
			//A*
			return ((lhs.fCost) >= (rhs.fCost));
		}

		float PathSearch::getDistance(Tile* lhs, Tile* rhs)
		{
			float x1 = lhs->getXCoordinate();
			float y1 = lhs->getYCoordinate();
			float x2 = rhs->getXCoordinate();
			float y2 = rhs->getYCoordinate();
			float distance = (std::pow(x2 - x1, 2)) + (std::pow(y2 - y1, 2));
			return std::sqrt(distance);
		}
	}
}  // close namespace ufl_cap4053::searches