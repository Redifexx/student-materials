#include <iterator>
#include <iostream>
using namespace std;

// LinkedList class should go in the "ufl_cap4053::fundamentals" namespace!
namespace ufl_cap4053 {
	namespace fundamentals {
		template <typename T>
		class LinkedList {
		private:
			struct Node
			{
				Node()
				{
					data = "";
					next = nullptr;
					prev = nullptr;
				};
				T data;
				Node* next;
				Node* prev;
			};
			int nodeCount;
		public:
			Node* head;
			Node* tail;

			class Iterator {
			public:
				Node* cur;

				Iterator()
				{
					cur = nullptr;
				};

				T operator*() const
				{ 
					return cur->data;
				};

				Iterator& operator++()
				{	
					if (cur)
					{
						if (cur->next == nullptr)
						{
							cur = nullptr;
						}
						else
						{
							cur = cur->next;
						}
					}
					return *this;
				};

				bool operator==(Iterator const& rhs)
				{
					
					if (rhs.cur == nullptr)
					{
						return (cur == nullptr);
					}
					else
					{
						return (cur->data == rhs.cur->data);
					}
					
				};

				bool operator!=(Iterator const& rhs)
				{
					
					if (rhs.cur == nullptr)
					{
						return (cur != nullptr);
					}
					else
					{
						return (cur->data != rhs.cur->data);
					}
				};

			};

			LinkedList<T>()
			{
				nodeCount = 0;
			};

			~LinkedList<T>()
			{
				delete head;
				delete tail;
			};

			Iterator begin() const
			{
				Iterator temp;
				temp.cur = head;
				return temp;
			};

			Iterator end() const
			{
				Iterator temp;
				temp.cur = nullptr;
				return temp;
			};

			bool isEmpty() const
			{
				return (nodeCount == 0);
			};

			T getFront() const
			{
				return head->data;
			};

			T getBack() const
			{
				return tail->data;
			};

			void enqueue(T element)
			{
				if (isEmpty())
				{
					delete head;
					head = new Node;
					head->data = element;
					tail = head;
				}
				else
				{
					Node* temp = tail;
					tail = new Node;
					tail->data = element;
					temp->next = tail;
					tail->prev = temp;
				}
				nodeCount++;
			};

			void dequeue()
			{
				if (!isEmpty())
				{
					Node* temp = head;
					if (head->next != nullptr)
					{
						head->next->prev = nullptr;
						head = head->next;
						delete temp;
					}
					else
					{
						delete temp;
						head = new Node;
						head->data = "";
						tail = head;
					}
					nodeCount--;
				}
			};

			void pop()
			{
				if (!isEmpty())
				{
					Node* temp = tail;
					if (tail->prev != nullptr)
					{
						tail->prev->next = nullptr;
						tail = tail->prev;
						delete temp;
					}
					else
					{
						delete temp;
						tail = new Node;
						tail->data = "";
						head = tail;
					}
					nodeCount--;
				}
			};

			void clear()
			{
				if (!isEmpty())
				{
					while (!isEmpty())
					{
						pop();
					}
				}
			};

			bool contains(T element) const
			{
				Node* temp = head;
				while (temp != nullptr)
				{
					if (temp->data == element)
					{
						return true;
					}
					else
					{
						temp = temp->next;
					}
				}
				return false;
			};

			void remove(T element)
			{
				Node* temp = head;
				while (temp != nullptr)
				{
					if (temp->data == element)
					{
						if (temp == head)
						{
							dequeue();
						}
						else if (temp == tail)
						{
							pop();
						}
						else
						{
							temp->next->prev = temp->prev;
							temp->prev->next = temp->next;
							delete temp;
							nodeCount--;
						}
						break;
					}
					else
					{
						temp = temp->next;
					}
				}
			};
		};
	} 
}
