using NUnit.Framework;
using Trust;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace TrustTest
{
    [TestFixture]
    public class TrustTest
    {
        // Todas la pruebas las haremos con estas listas.
        public static IEnumerable<TestCaseData> TestList
        {
            get
            {

                var list1 = Enumerable.Range(1, 10)
                      .Select(n => n.ToString())
                      .ToList();

                var list2 = Enumerable.Range(1, 100)
                     .Select(n => n.ToString())
                     .ToList();

                var list3 = Enumerable.Range(1, 1000)
                     .Select(n => n.ToString())
                     .ToList();

                yield return new TestCaseData(new List<string>());
                yield return new TestCaseData(list1);
                yield return new TestCaseData(list2);
                yield return new TestCaseData(list3);

            }
        }

        // Si este test tarda más de un minuto, dará error de timeout
        [TestCaseSource("TestList")]
        [Timeout (60000)]
        public void Consume_WithVariousConsumers_ShouldConsumeTheQueueCorrectly(List<string> ls)
        {
            var sq = StringQueue.CreateStringQueue(ls);

            // Necesitamos usar Task.Factory.StartNew para probar nuestro método Consume
            // durante un cierto tiempo (5 segundos). Este wrapper por sí mismo no garantiza un nuevo
            // hilo, pero nuestro método se encarga de crearlo, tal como se indica en los requerimientos.
            // La diferencia principal es que Task.Factory usa Thread-pool, que es manejado por el
            // sistema, se supone que de forma más eficiente. También permite especificar un intervalo 
            // de tiempo en el que debe correr el proceso, como en este caso.
            // Probar procesos multi-threading siempre es un poco
            // truculento, pero es mucho mejor que no hacer nada.

            var t1 = Task.Factory.StartNew(() => Consumer.CreateConsumerNewThread(sq, "Consumer 1").Consume()).Wait(5000);
            var t2 = Task.Factory.StartNew(() => Consumer.CreateConsumerNewThread(sq, "Consumer 2").Consume()).Wait(5000);
            var t3 = Task.Factory.StartNew(() => Consumer.CreateConsumerNewThread(sq, "Consumer 3").Consume()).Wait(5000);
            var t4 = Task.Factory.StartNew(() => Consumer.CreateConsumerNewThread(sq, "Consumer 4").Consume()).Wait(5000);

            Assert.AreEqual(sq.Count(), 0);
        }

        [TestCaseSource("TestList")]
        public void CreateStringQueue_FromStringList_ShouldCreateAThreadSafeConcurrentQueueWithSameElements(List<string> ls)
        {
            // Compare just the total count and the first item.

            var sq = StringQueue.CreateStringQueue(ls);
            Assert.AreEqual(sq.Count(), ls.Count);

            var item = sq.Dequeue();
            Assert.AreEqual(item, ls.FirstOrDefault());
        }

        [TestCase (null)]
        public void CreateStringQueue_FromNullStringList_ShouldThrowArgumentNUllException(List<string> ls)
        {
            Assert.Throws<ArgumentNullException>(() => StringQueue.CreateStringQueue(ls));
        }

        [TestCaseSource("TestList")]
        public void Enqueue_CorrectStringItem_ShouldAddItemToTheQueueAndTriggersEventItemEnqueued(List<string> ls)
        {
            var sq = StringQueue.CreateStringQueue(ls);

            var wait1 = new AutoResetEvent(false);
            sq.ItemEnqueued += (sender, eventArgs) => wait1.Set();
           
            sq.Enqueue("test");

            Assert.IsTrue(wait1.WaitOne(TimeSpan.FromSeconds(5)));
            Assert.AreEqual(sq.Count(), ls.Count + 1);
        }

        [TestCaseSource("TestList")]
        public void Enqueue_NullItem_ShouldReturnTheQueueUnchangedAndNotTriggersEventItemEnqueued(List<string> ls)
        {
            var sq = StringQueue.CreateStringQueue(ls);

            var wait1 = new AutoResetEvent(false);
            sq.ItemEnqueued += (sender, eventArgs) => wait1.Set();

            sq.Enqueue(null);

            Assert.IsFalse(wait1.WaitOne(TimeSpan.FromSeconds(5)));
            Assert.AreEqual(sq.Count(), ls.Count);
        }

        [TestCaseSource("TestList")]
        public void Dequeue_Item_ShouldUpdateTheQueueAndTriggersEventItemDequeued(List<string> ls)
        {
            var sq = StringQueue.CreateStringQueue(ls);

            var wait1 = new AutoResetEvent(false);
            sq.ItemDequeued += (sender, eventArgs) => wait1.Set();

            var item = sq.Dequeue();

            if (item != null)
            {
                Assert.IsTrue(wait1.WaitOne(TimeSpan.FromSeconds(5)));
            } else
            {
                Assert.IsFalse(wait1.WaitOne(TimeSpan.FromSeconds(5)));
            }
           
            Assert.AreEqual(item, ls.FirstOrDefault());
        }
    }
}
