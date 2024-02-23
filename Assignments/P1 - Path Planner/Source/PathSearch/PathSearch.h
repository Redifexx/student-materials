#include "../platform.h" // This file will make exporting DLL symbols simpler for students.
#include <vector>
#include <queue> //for BFS
#include <unordered_map>
#include <unordered_set>
#include <chrono>
#include <iostream>
#include "../Framework/TileSystem/Tile.h"
#include "../Framework/TileSystem/TileMap.h"

namespace ufl_cap4053
{
	namespace searches
	{
		class PathSearch
		{
			TileMap* tileMap;
			bool areAdjacent(const Tile* lhs, const Tile* rhs);
			std::vector<Tile*> getAdjacent(int _r, int _c);
			std::unordered_map<Tile*, std::vector<Tile*>> searchGraph;
			std::queue<Tile*> searchQueue;
			std::unordered_set<Tile*> visitedTiles;
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
