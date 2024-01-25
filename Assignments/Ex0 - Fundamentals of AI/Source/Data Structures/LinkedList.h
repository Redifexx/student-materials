#include <iterator>


// LinkedList class should go in the "ufl_cap4053::fundamentals" namespace!
namespace ufl_cap4053 {
	namespace fundamentals {
		template <typename T>
		class Node
		{
			T data;
			Node* front;
			Node* back;
		};

		template <typename T>
		class Iterator {
			T operator*() const {};
			Iterator& operator++() {};
			bool operator==(Iterator const& rhs) {};
			bool operator!=(Iterator const& rhs) {};
		};


		template <typename T>
		class LinkedList {
		
		public:
			LinkedList<T>();
			Iterator begin() const {};
			Iterator end() const {};
			bool isEmpty() const {};
			T getFront() const {};
			T getBack() const {};
			void enqueue(T element) {};
			void dequeue() {};
			void pop() {};
			void clear() {};
			bool contains(T element) const {};
			void remove(T element) {};
		};
	} 
}
