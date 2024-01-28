#include <vector>
#include <queue>
using namespace std;

// TreeNode class should go in the "ufl_cap4053::fundamentals" namespace!
namespace ufl_cap4053 {
	namespace fundamentals {
		template <typename T>
		class TreeNode 
		{
		private:
			T data;
			vector<TreeNode<T>*> children;
		public:

			TreeNode<T>() 
			{
				data = "";
			};

			TreeNode<T>(T element) 
			{
				data = element;
			};

			~TreeNode<T>()
			{
				for (int i = 0; i < children.size(); i++)
				{
					delete children.at(i);
				}
			};

			const T& getData() const 
			{
				return data;
			};

			size_t getChildCount() const 
			{
				return children.size();
			};

			TreeNode<T>* getChild(size_t index) 
			{
				return children.at(index);
			};

			TreeNode<T>* getChild(size_t index) const 
			{
				return children.at(index);
			};

			void addChild(TreeNode<T>* child) 
			{
				children.push_back(child);
			};

			TreeNode<T>* removeChild(size_t index) 
			{
				TreeNode<T>* temp = children.at(index);
				children.erase(children.begin() + index);
				return temp;
			};

			void breadthFirstTraverse(void (*dataFunction)(const T)) const 
			{
				queue<const TreeNode<T>*> curQueue;
				const TreeNode<T>* curNode = this;
				curQueue.push(curNode);
				while (!curQueue.empty())
				{
					curNode = curQueue.front();
					curQueue.pop();
					dataFunction(curNode->data);

					for (int i = 0; i < curNode->getChildCount(); i++)
					{
						curQueue.push(curNode->getChild(i));
					}
				}
			};

			void preOrderTraverse(void (*dataFunction)(const T)) const
			{
				dataFunction(this->data);
				for (int i = 0; i < this->getChildCount(); i++)
				{
					TreeNode<T>* curChild = this->getChild(i);
					if (curChild != nullptr)
					{
						curChild->preOrderTraverse(dataFunction);
					}
				} 
			};

			void postOrderTraverse(void (*dataFunction)(const T)) const
			{
				for (int i = 0; i < this->getChildCount(); i++)
				{
					TreeNode<T>* curChild = this->getChild(i);
					if (curChild != nullptr)
					{
						curChild->postOrderTraverse(dataFunction);
					}
				}
				dataFunction(this->data);
			};
		};
}}
