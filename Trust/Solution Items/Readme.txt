DELIVERABLES
- Source code of your application
REQUIREMENTS
- The application will be implemented in .NET version 3.5 or higher.
TASK FORMULATION
Implement a console application that will be composed of a fifo queue of strings and two
threads fighting to dequeue from the queue.
QUEUE SPECIFICATIONS:
- The queue will be a wrapper of a List<string> generic collection. It will have the
following public methods:
o public void Enqueue(string item): Add an item to the list
o public string Dequeue(): Return and remove the last item of the list
o public int Count(): Get the number of items in the list
- When an element is enqueued, the event OnItemEnqueued will be fired.
- Remember that List<T> is not thread-safe.
FIGHTING THREADS
- Create two instances of System.Thread named “t1” and “t2” executing the
following method:
o public string Consume(): This method will have an inifinite loop that will
dequeue elements from the queue until it is empty. Every time an element
is dequeued it will be printed in the console along with the name of the
thread. Once the queue is empty, the method will wait to receive a signal to
start dequeueing elements again. The signal will be fired by the
implementation of the event OnItemEnqueued. This signal should start all
sleeping threads.
TESTING METHOD
- You may want to write a method in the main process to populate the queue.
EVALUATION GOALS
- Neat and quality code
- Usage of .NET multithreading namespaces