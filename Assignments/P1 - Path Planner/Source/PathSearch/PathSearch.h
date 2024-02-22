#include "../platform.h" // This file will make exporting DLL symbols simpler for students.
#include <vector>
#include "../Framework/TileSystem/Tile.h"
#include "../Framework/TileSystem/TileMap.h"

namespace ufl_cap4053
{
	namespace searches
	{
		class PathSearch
		{
		// CLASS DECLARATION GOES HERE
			public:
				DLLEXPORT PathSearch(); // EX: DLLEXPORT required for public methods - see platform.h
				PathSearch();
				~PathSearch();
				void load(TileMap* _tileMap);
				void initialize(int startRow, int startCol, int goalRow, int goalCol);
				void update(long timeslice);
				void shutdown();
				void unload();
				bool isDone() const;
				std::vector<Tile* const> const getSolution() const;
		};
	}
}  // close namespace ufl_cap4053::searches
