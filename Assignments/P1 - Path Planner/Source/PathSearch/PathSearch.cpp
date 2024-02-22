#include "PathSearch.h";

namespace ufl_cap4053
{
	namespace searches
	{
		// CLASS DEFINITION GOES HERE
		PathSearch::PathSearch() {}
		PathSearch::~PathSearch() {}
		void PathSearch::load(TileMap* _tileMap) {}
		void PathSearch::initialize(int startRow, int startCol, int goalRow, int goalCol) {}
		void PathSearch::shutdown() {}
		void PathSearch::unload() {}
		bool PathSearch::isDone() const {}
		std::vector<Tile* const> const getSolution() {}
	}
}  // close namespace ufl_cap4053::searches