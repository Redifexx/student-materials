#include "../platform.h" // This file will make exporting DLL symbols simpler for students.
#include <vector>
#include <stack> //To Display Path
#include <unordered_map>
#include <unordered_set>
#include <chrono>
#include <iostream>
#include <cmath>
#include "../Framework/TileSystem/Tile.h"
#include "../Framework/TileSystem/TileMap.h"
#include "../PriorityQueue.h"

namespace ufl_cap4053
{
	namespace searches
	{
		class PathSearch
		{
			TileMap* tileMap;
			bool areAdjacent(const Tile* lhs, const Tile* rhs);
			std::vector<Tile*> getAdjacent(int _r, int _c);
			class TileNode 
			{
			public:
				Tile* tile;
				TileNode* parent;
				float givenCost;
				float hCost;
				float hWeight;
				float fCost;
				std::vector<Tile*> adjacentTiles;
				unsigned char weight;
				TileNode()
				{
					tile = nullptr;
					weight = 0;
					hWeight = 0.5f;
					parent = nullptr;
					givenCost = 0;
					hCost = 0;
					fCost = 0;
				}
				TileNode(Tile* _tile)
				{
					tile = _tile;
					weight = _tile->getWeight();
					hWeight = 0.5f;
					parent = nullptr;
					givenCost = 0;
					hCost = 0;
					fCost = 0;
				}
				void updateFCost()
				{
					fCost = givenCost + (hWeight * hCost);
				}
			};
			TileNode* root;

			std::unordered_map<Tile*, TileNode*> searchGraph;

			PriorityQueue<TileNode>* searchQueue;


			std::vector<double> elapsedTimes;

			std::unordered_set<Tile*> openTiles;
			std::unordered_set<Tile*> visitedTiles;

			bool _isDone;

			std::stack<TileNode*> pathStack;

			std::vector<Tile const*> solution;

			static bool isGreaterThan(TileNode const& lhs, TileNode const& rhs);
			float getDistance(Tile* lhs, Tile* rhs);

			// CLASS DECLARATION GOES HERE
			public:
				DLLEXPORT PathSearch(); // EX: DLLEXPORT required for public methods - see platform.h
				DLLEXPORT ~PathSearch();
				DLLEXPORT void load(TileMap* _tileMap);
				DLLEXPORT void initialize(int startRow, int startCol, int goalRow, int goalCol);
				DLLEXPORT void update(long timeslice);
				DLLEXPORT void shutdown();
				DLLEXPORT void unload();
				DLLEXPORT bool isDone() const;
				DLLEXPORT std::vector<Tile const*> const getSolution() const;
		};
	}
}  // close namespace ufl_cap4053::searches
