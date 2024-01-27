#include <iterator>


// LinkedList class should go in the "ufl_cap4053::fundamentals" namespace!
namespace ufl_cap4053 {
	namespace fundamentals {
		template <typename T>
		class LinkedList {
		private:
			struct Node
			{
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

				T operator*() const
				{
					return cur->data;
				};

				Iterator& operator++()
				{	
					if (cur->next != nullptr)
					{
						cur = cur->next;
					}
					return *this;
				};

				bool operator==(Iterator const& rhs)
				{
					return (cur == rhs.cur);
				};

				bool operator!=(Iterator const& rhs)
				{
					return (cur == rhs.cur);
				};

			};

			LinkedList<T>()
			{
				nodeCount = 0;
			};

			Iterator begin() const
			{
				Iterator* itr = new Iterator;
				itr->cur = head;
				return *itr;
			};

			Iterator end() const
			{
				Iterator* itr = new Iterator;
				itr->cur = tail;
				return *itr;
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
					}
					delete temp;
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
					}
					delete temp;
					nodeCount--;
				}
			};

			void clear()
			{
				if (!isEmpty())
				{
					while (head != tail)
					{
						pop();
					}
					delete head;
					nodeCount = 0;
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
							temp->next->prev = temp->prev->next;
							temp->prev->next = temp->next->prev;
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
