#include "PathSearch.h";

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

				curTile = tileMap->getTile(_r - 1, _c + 1); //LeftDown
				if (curTile != nullptr)
				{
					adjacentTiles.push_back(curTile);
				}
			}
			else
			{
				curTile = tileMap->getTile(_r + 1, _c - 1); //RightUp
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
		{}
		PathSearch::~PathSearch()
		{}

		void PathSearch::load(TileMap* _tileMap)
		{
			tileMap = _tileMap;
			//Loads the tiles into an unordered map that'll be treated like nodes
			for (int r = 0; r < tileMap->getRowCount(); r++)
			{
				for (int c = 0; c < tileMap->getColumnCount(); c++)
				{
					Tile* curTile = tileMap->getTile(r, c);
					searchGraph[curTile] = getAdjacent(r, c);
				}
			}
		}

		//Sets the start and goal tiles
		void PathSearch::initialize(int startRow, int startCol, int goalRow, int goalCol)
		{
			tileMap->setStartTile(startRow, startCol);
			tileMap->setGoalTile(goalRow, goalCol);
			searchQueue.push(tileMap->getStartTile());
			visitedTiles.insert(tileMap->getStartTile());
		}

		//Runs the algorithm
		void PathSearch::update(long timeslice)
		{
			auto startTime = std::chrono::high_resolution_clock::now();
			auto endTime = startTime + std::chrono::milliseconds(timeslice);
			int iterations = -1;
			if (timeslice == 0)
			{
				iterations == 1; //Dummy int to stop the while
			}
			//while (((std::chrono::high_resolution_clock::now() < endTime) || (iterations == 1)) && !searchQueue.empty())
			if (!searchQueue.empty())
			{
				Tile* curTile = searchQueue.front();
				searchQueue.pop();

				//PathPlanner Stuff
				if (curTile == tileMap->getGoalTile())
				{
					curTile->setFill(0xFF00FF00);
					//break;
				}


				for (Tile* adjacent : searchGraph[curTile])
				{
					if (visitedTiles.find(adjacent) == visitedTiles.end())
					{
						searchQueue.push(adjacent);
						visitedTiles.insert(adjacent);
					}
				}
				//curTile->setFill(0x7F00FF00);
				iterations++;
				std::cout << "ITERATION" << std::endl;
			}
			std::cout << "done" << std::endl;
		}

		void PathSearch::shutdown() {}
		void PathSearch::unload() {}
		bool PathSearch::isDone() const
		{
			return false;
		}
		std::vector<Tile const*> const PathSearch::getSolution() const
		{
			std::vector<Tile const*> tiles;
			return tiles;
		}
	}
}  // close namespace ufl_cap4053::searches