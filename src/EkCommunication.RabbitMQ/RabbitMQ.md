# RabbitMQ

> [RabbitMQ官方文档](https://www.rabbitmq.com/admin-guide.html)

### 介绍

> RabbitMQ 是一个消息代理：它接受和转发消息。您可以将其视为邮局：当您将要邮寄的邮件放入邮箱时，您可以确定邮递员最终会将邮件递送给您的收件人。在这个类比中，RabbitMQ 是一个邮箱、一个邮局和一个邮递员。
>
> RabbitMQ 和邮局之间的主要区别在于它不处理纸张，而是接受、存储和转发二进制数据块 -*消息*。
>
> RabbitMQ 和一般的消息传递使用一些行话。
>
> - *生产*无非就是发送。发送消息的程序是*生产者*。
> - *队列*是位于 RabbitMQ 中的邮箱的名称。尽管消息流经 RabbitMQ 和您的应用程序，但它们只能存储在*队列*中。*队列*仅受主机的内存和磁盘限制，它本质上是一个大的消息缓冲区。许多*生产者*可以发送去一个队列的消息，许多*消费者*可以尝试从一个*队列*接收数据。
> - *消费*与接收具有相似的含义。*消费者*是一个主要等待接收消息的程序。

##### 概括

- 客户端

  > 1. 当客户端启动时，它会创建一个回调队列。
  > 2. 对于 **RPC** 请求，客户端发送具有两个属性的消息： **ReplyTo** 设置为 **回调队列** 名称 和 **CorrelationId ** 设置为对应请求的唯一值。
  > 3. 请求被发送到 rpc_queue 队列。（ **ReplyTo** 值）
  > 4. RPC 工作者（又名：服务器）正在等待该队列上的请求。当出现请求时，它会执行该工作并使用来自ReplyTo属性的队列将带有结果的消息发送回客户端。
  > 5. 客户端等待回调队列中的数据。出现消息时，它会检查 **CorrelationId** 属性。如果它与请求中的值匹配，则将响应返回给应用程序。

- 服务端

  > 1. 建立一个连接和通道，并声明一个专有的 **“callback”** 队列以进行回复。
  > 2. 订阅 **“callback”** 队列，以便我们可以接收 RPC 响应。
  > 3. 发出实际的 RPC 请求。
  > 4. 首先生成一个唯一的 **CorrelationId**  编号并将其保存以在它到达时识别适当的响应。
  > 5. 发布请求消息，其中包含两个属性： **ReplyTo** 和 **CorrelationId**。
  > 6. 等待正确的响应到来。
  > 7. 对于每条响应消息，客户端都会检查 **CorrelationId**  是否是正在寻找的消息。如果是这样，保存响应。
  > 8. 最后，将响应返回给用户。

### 创建服务器链接

```c#
var factory = new ConnectionFactory() { HostName = "localhost" };
using(var connection = factory.CreateConnection())// 连接到 RabbitMQ 节点
    using (var channel = connection.CreateModel())// 创建通道
        {
            // 声明一个队列 声明后不允许 使用不同参数重新定义现有队列
            // 并且会向任何尝试这样做的程序返回错误
            channel.QueueDeclare(queue: "hello",// 队列名称
                durable: false,// 标记是否将队列持久化 
                exclusive: false,
                autoDelete: false,
                arguments: null);
                
            //#region 发送与接受消息
            ...
            //#endregion
		}
```

### 发送消息

##### 发送消息

```c#
string message = "Hello World!" + msg;
var body = Encoding.UTF8.GetBytes(message);

//#region 通道预取设置

// 设置在工作者 处理完前不要发送新数据
// 注意事项
// 如果所有工作人员都很忙，队列可能会被填满。
// 可增加更多的工人，或者有一些其他的策略。
channel.BasicQos(
    prefetchSize: 0,// 预读取大小
    prefetchCount: 1,// 预读取数量
    global: false);// 全局化
//#endregion

// 创建消息属性
var properties = channel.CreateBasicProperties();
properties.Persistent = true;// 标记是否将消息持久化

//#region 接收回复消息
properties.ReplyTo = "replyQueueName";// 回复路由键
var correlationId = Guid.NewGuid().ToString();
properties.CorrelationId = correlationId;//  RPC 响应与请求关联ID


var consumer = new EventingBasicConsumer(channel);
consumer.Received += (model, ea) => {
    var body = ea.Body.ToArray();
    var response = Encoding.UTF8.GetString(body);
    if (ea.BasicProperties.CorrelationId == correlationId) {
        Console.WriteLine($"收到回复消息{response}");
    }
};
// 设置交付确认
channel.BasicConsume(
    consumer: consumer,
    queue: "replyQueueName",
    autoAck: true);
//#endregion
    
channel.BasicPublish(exchange: "",// 交换器
    routingKey: "hello",// 路由键
    basicProperties: properties,
    body: body);
```

##### 通道预处理设置（QoS）

> 因为消息是异步发送（推送）到客户端的，所以在任何给定时刻，通道上通常都会有多个“正在运行”的消息。此外，来自客户端的手动确认本质上也是异步的。所以有一个未确认的交付标签的滑动窗口。
>
> 开发人员通常更愿意限制此窗口的大小，以避免消费者端的无界缓冲区问题。
>
> 这是通过使用  **BasicQos** 方法设置“预取计数”值来完成的。该值定义了通道上允许的未确认传递的最大数量。一旦数量达到配置的数量，**RabbitMQ** 将停止在通道上传递更多消息，除非至少有一个未完成的消息被确认。（值为0被视为无限，允许任意数量的未确认消息。）

```c#
// 设置在工作者 处理完前不要发送新数据
// 注意事项
// 如果所有工作人员都很忙，队列可能会被填满。
// 可增加更多的工人，或者有一些其他的策略。
channel.BasicQos(
    prefetchSize: 0,// 预读取大小
    prefetchCount: 1,// 预读取数量
    global: false);// 全局化
```

> 例如，假设通道Ch上有未确认的交付标签 5、6、7 和 8，并且通道 Ch 的预取计数设置为 4，RabbitMQ 将不会在Ch上推送更多交付，除非至少有一个未完成交付被承认。
>
> 当确认帧到达该通道时， delivery_tag设置为5（或6、7或8），RabbitMQ 将注意到并再发送一条消息。一次确认[多条消息](https://www.rabbitmq.com/confirms.html#consumer-acks-multiple-parameter) 将使多条消息可用于传递。

> 值得重申的是，交付流程和手动客户端确认是完全异步的。因此，如果预取值在已经有交付的情况下更改，则会出现自然的竞争条件，并且通道上可能暂时存在多于预取计数的未确认消息。

- 每通道、每消费者和全局预取

  > 可以为特定通道或特定消费者配置 QoS 设置。[消费者预取](https://www.rabbitmq.com/consumer-prefetch.html)指南解释了这个范围的影响。

- 预取和轮询消费者

  > QoS 预取设置对使用 BasicGet （“pull API”）获取的消息没有影响，即使在手动确认模式下也是如此。

##### 消费者确认模式、预取和吞吐量

> 确认模式和 QoS 预取值对消费者吞吐量有显着影响。一般来说，增加预取将提高消息传递给消费者的速度。自动确认模式可产生最佳的交付率。但是，在这两种情况下，已传递但尚未处理的消息的数量也会增加，从而增加消费者 RAM 消耗。

> **应谨慎使用自动确认模式或无限制预取的手动确认模式。**
>
> 消费者在没有确认的情况下消费了大量消息，将导致他们连接的节点上的内存消耗增长。找到合适的预取值是一个反复试验的问题，并且会因工作负载而异。100 到 300 范围内的值通常提供最佳吞吐量，并且不会冒压倒消费者的重大风险。较高[的值经常会遇到收益递减规律](https://blog.rabbitmq.com/posts/2014/04/finding-bottlenecks-with-rabbitmq-3-3/)。

> 预取值 1 是最保守的。它将显着降低吞吐量，特别是在消费者连接延迟很高的环境中。对于许多应用程序，较高的值将是适当和最佳的。

##### 当消费者失败或失去连接时：自动重新排队

> 当使用手动确认时，任何未确认的传递（消息）都会在传递发生的通道（或连接）关闭时自动重新排队。这包括客户端的 TCP 连接丢失、消费者应用程序（进程）故障和通道级协议异常（如下所述）。

> [请注意，检测不可用的客户端](https://www.rabbitmq.com/heartbeats.html)需要一段时间。
>
> 由于这种行为，消费者必须准备好处理重新交付，否则在实施时要牢记[幂等](https://en.wikipedia.org/wiki/Idempotence)性。Redeliveries 将有一个特殊的布尔属性redeliver ， 由 RabbitMQ设置为true 。对于首次交付，它将设置为false。请注意，消费者可以接收以前传递给另一个消费者的消息。

##### 出版商确认

> 网络可能会以不太明显的方式出现故障，并且检测某些故障[需要时间](https://www.rabbitmq.com/heartbeats.html)。因此，将一个协议帧或一组帧（例如，发布的消息）写入其套接字的客户端不能假定该消息已到达服务器并已成功处理。它可能在途中丢失，或者它的交付可能会大大延迟

> 使用标准 AMQP 0-9-1，保证消息不丢失的唯一方法是使用事务——使通道具有事务性，然后为每条消息或一组消息发布、提交。在这种情况下，交易是不必要的重量级，并将吞吐量降低了 250 倍。为了解决这个问题，引入了确认机制。它模仿了协议中已经存在的消费者确认机制。

> 为了启用确认，客户端发送 confirm.select方法。根据是否 设置了no-wait，代理可能会使用confirm.select-ok进行响应。
>
> 一旦在通道上使用了 confirm.select方法，就说它处于确认模式。交
>
> 易通道不能进入确认模式，一旦通道处于确认模式，就不能进行交易。
>
> 一旦通道处于确认模式，代理和客户端都会计算消息（在第一个 confirm.select上从 1 开始计数）。然后，代理在处理消息时通过在同一通道上发送 basic.ack来确认消息。delivery-tag字段包含已确认消息的 序列号。代理还可以 在basic.ack中设置multiple字段，以指示所有消息（包括具有序列号的消息）都已被处理。

##### 发布失败

> 在代理无法成功处理消息的特殊情况下，代理将发送basic.nack而不是basic.ack。在这种情况下，basic.nack 的字段与basic.ack中 的相应字段含义相同，应忽略requeue字段。通过处理一条或多条消息，代理表明它无法处理这些消息并拒绝对它们负责；此时，客户端可能会选择重新发布消息。

> 通道进入确认模式后，所有后续发布的消息都将被确认或确认一次。不保证消息在多长时间内得到确认。没有消息将被确认和确认。

> 只有当负责队列的 Erlang 进程发生内部错误时，才会传递basic.nack 。

##### 发布的消息什么时候会被 **Broker** 确认？

> 对于不可路由的消息，一旦交换验证消息不会路由到任何队列（返回队列的空列表），代理将发出确认。如果消息也是强制发布的，则basic.return在basic.ack之前发送给客户端。否定确认 ( basic.nack ) 也是如此。

> 对于可路由消息，basic.ack在消息被所有队列接受时发送。对于路由到持久队列的持久消息，这**意味着持久化到磁盘**。For [quorum queues](https://www.rabbitmq.com/quorum-queues.html) , this means that a quorum replicas have accepted and confirmed the message to the elected leader.

##### 最大交付标签

> 交付标签是一个 64 位长的值，因此它的最大值是9223372036854775807。由于交付标签的范围是每个渠道，因此发布者或消费者在实践中不太可能超出此值。

##### 异常处理

> **发送无效**
>
> 也许代理启动时没有足够的可用磁盘空间（默认情况下它需要至少 50 MB 可用空间），因此拒绝接受消息。检查代理日志文件以确认并在必要时减少限制。配置[文件文档](https://www.rabbitmq.com/configure.html#config-items)将向您展示如何设置disk_free_limit。

### 接收消息

##### 接收消息

```c#
var consumer = new EventingBasicConsumer(channel);
// 接收完成事件
consumer.Received += (model, ea) => {
    var body = ea.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Thread.Sleep(1000);
    Console.WriteLine(" [x] Received {0}  {1}", message, ClientId);
    //#region 接受消息回复确认
    
    // 两种方式配置确认交付完成，完成后消息将被丢弃
    // ((EventingBasicConsumer)model).Model.BasicAck(
    //     deliveryTag: ea.DeliveryTag,
    //     multiple: false);
    channel.BasicAck(
        deliveryTag: ea.DeliveryTag, // 交付标识
        multiple: false);// 设置为 true 时,RabbitMQ 将确认该渠道所有未完成的交付标签
    
    //#endregion
};
//#region 设置交付确认
channel.BasicConsume(queue: "hello",
    autoAck: fasle,
    consumer: consumer);
//#endregion
```

- > 接受消息将获取  EventingBasicConsumer 和 BasicDeliverEventArgs

  BasicDeliverEventArgs.DeliveryTag  **本次消息的交付标签**

  > 交付标签的范围是每个渠道
  >
  > 交付标签是单调增长的正整数，并由客户端库呈现。
  >
  > 确认交付的客户端库方法将交付标签作为参数。
  >
  > 由于交付标签的范围是每个渠道，因此必须在接收交付的同一渠道上确认交付。
  >
  > 在不同的通道上确认将导致“未知传递标签”协议异常并关闭通道

##### 客户端错误

- 双重确认和未知标签

  > 如果客户端多次确认同一个传递标签，RabbitMQ 将导致通道错误，
  >
  > 例如**PRECONDITION_FAILED - unknown delivery tag 100**。如果使用了未知的传递标签，将引发相同的通道异常。
  >
  > 代理会提示**“未知传递标签”**的另一种情况是，在与接收传递的通道不同的通道上尝试确认，无论是肯定的还是否定的。必须在同一渠道上确认交付。

##### 设置交付确认

```c#
channel.BasicConsume(queue: "hello",
    autoAck: false,
    consumer: consumer);
```

- > 当节点将消息传递给消费者时，它必须决定是否应将消息视为由消费者处理（或至少接收）。
  >
  > 由于多个事物（客户端连接、消费者应用程序等）可能会失败，因此此决定是数据安全问题。
  >
  > 消息传递协议通常提供一种确认机制，允许消费者确认传递到他们连接的节点。
  >
  > 是否使用该机制在消费者**订阅时**决定。

##### 确认模式

> 用于肯定确认
>
> ```c#
> IModel.BasicAck(ulong deliveryTag, bool multiple);
> ```
>
> 用于否定确认（注意：这是[RabbitMQ 对 AMQP 0-9-1 的扩展](https://www.rabbitmq.com/nack.html)）
>
> ```c#
> IModel.BasicNack(ulong deliveryTag, bool multiple, bool requeu);
> ```
>
> 用于否定确认，但与basic.nack相比有一个限制
>
> ```c#
> IModel.BasicReject(ulong deliveryTag, bool requeue);
> ```

- 肯定确认只是指示 RabbitMQ 将消息记录为已传递并且可以被丢弃。带有basic.reject的否定确认具有相同的效果。区别主要在于语义上：肯定的确认假定消息已成功处理，而否定的对应则表明未处理但仍应删除传递。
- 在自动确认模式下，消息在发送后立即被视为成功传递。这种模式以更高的吞吐量（只要消费者能够跟上）以降低交付和消费者处理的安全性为代价。这种模式通常被称为“即发即弃”。与手动确认模型不同的是，如果消费者的 TCP 连接或通道在发送成功之前关闭，服务器发送的消息将丢失。因此，自动消息确认**应该被认为是不安全** 的，并且不适合所有工作负载。
- 使用自动确认模式时需要考虑的另一件事是消费者过载。手动确认模式通常与有界通道预取一起使用，这限制了通道上未完成（“正在进行”）交付的数量。然而，对于自动确认，根据定义没有这样的限制。因此，消费者可能会对交付速度感到不知所措，可能会在内存中累积积压并耗尽堆或让他们的进程被操作系统终止。一些客户端库将应用 TCP 背压（停止从套接字读取，直到未处理交付的积压下降超过某个限制）。因此，自动确认模式仅推荐给能够以稳定的速度高效处理交付的消费者。

##### 接收消息确认

```c#
// 两种方式配置确认交付完成，完成后消息将被丢弃
// ((EventingBasicConsumer)model).Model.BasicAck(
//     deliveryTag: ea.DeliveryTag,
//     multiple: false);
channel.BasicAck(
    deliveryTag: ea.DeliveryTag, // 交付标识
    multiple: false);// 设置为 true 时,RabbitMQ 将确认该渠道所有未完成的交付标签
```

- > 忘记配置 BasicAck 将导致消息被重新传递( 可能看起来像随机重新传递 ) 
  > 但是 RabbitMQ 将消耗越来越多的内存，因为它无法释放任何未确认的消息。
  
- > 使用 **BasicConsume** 设置自动确认后，不可进行手动确认，否则将无法获取后续消息

##### 一次确认多个交付

> 批量手动确认以减少网络流量
>
> 这是通过将确认方法的 **multiple** 字段（见上文）设置为true来完成的。
>
> basic.reject历史上没有该字段，这就是RabbitMQ 引入basic.nack作为协议扩展的原因。

> 当 **multiple** 字段设置为true时，RabbitMQ 将确认所有未完成的交付标签，包括确认中指定的标签。与其他与确认相关的所有内容一样，这是每个渠道的范围。
>
> 例如，假设渠道上有未确认的传递标签 5、6、7 和 8 ，当一个确认帧到达该渠道时，delivery_tag设置为8 ，multiple设置为true，从 5 到 8 的所有标签都将被确认. 
>
> 如果multiple设置为false，则交付 5、6 和 7 仍将未被确认。

##### 交付的是否确认和重新排队

> 有时消费者无法立即处理交付，但其他实例可能能够。在这种情况下，可能需要重新排队并让另一个消费者接收和处理它。basic.reject和basic.nack是用于此目的的两种协议方法。

> 这些方法通常用于否定确认交付。这样的交付可以被经纪人丢弃或死信或重新排队。
>
> 此行为由 **requeue** 字段控制。当该字段设置为true时，代理将使用指定的交付标签重新排队交付（或多个交付，稍后将解释）。或者，当此字段设置为false时，如果已配置，消息将被路由到[死信交换](https://www.rabbitmq.com/dlx.html)，否则将被丢弃。

> ```c#
> // 否定确认，消息将丢弃
> channel.BasicReject(ea.DeliveryTag, false);
> // 重新排队传递
> channel.BasicReject(ea.DeliveryTag, true); 
> ```

> 当一条消息被重新排队时，如果可能的话，它将被放置到它在队列中的原始位置。如果不是（由于多个消费者共享一个队列时来自其他消费者的并发传递和确认），则消息将被重新排队到更接近队列头的位置。

> 重新排队的消息可能会立即准备好重新传递，具体取决于它们在队列中的位置以及具有活动消费者的通道使用的预取值。这意味着，如果所有消费者由于临时条件而无法处理交付而重新排队，他们将创建重新排队/重新交付循环。就网络带宽和 CPU 资源而言，此类循环可能代价高昂。消费者实现可以跟踪重新传递的数量并永久拒绝消息（丢弃它们）或在延迟后安排重新排队。

> 可以使用 **BasicNack**  方法一次拒绝或重新排队多条消息。这就是它与 **BasicReject** 的区别。它接受一个附加参数，multiple。
>
> ```c#
>  // 将所有未确认的交付重新排队，直到此交付标签
>  channel.BasicNack(ea.DeliveryTag, true , true ); 
> ```

##### 确认模式订阅的注意事项

> 在大多数情况下，RabbitMQ 将按照发布的顺序向发布者确认消息（这适用于在单个通道上发布的消息）。但是，发布者确认是异步发出的，可以确认单个消息或一组消息。发出确认的确切时间取决于消息的传递模式（持久与瞬态）以及消息路由到的队列的属性（见上文）。也就是说，不同的消息可以被认为在不同的时间准备好确认。这意味着与它们各自的消息相比，确认可以以不同的顺序到达。如果可能，应用程序不应依赖于确认的顺序。

### 回调队列

> 通过 RabbitMQ 进行 RPC 很容易。客户端发送请求消息，服务器回复响应消息。为了接收响应，我们需要在请求中发送一个“回调”队列地址：
>
> ```c#
> var props = channel.CreateBasicProperties();
> props.ReplyTo = replyQueueName;// 回调队列名称
> 
> var messageBytes = Encoding.UTF8.GetBytes(message);
> channel.BasicPublish(exchange: "",// 交换器
>     routingKey: "rpc_queue",// 路由键
>     basicProperties: props,
>     body: messageBytes);
> // 然后用代码从callback_queue中读取响应信息 
> ```
>
> 

##### 消息属性

**ea.BasicProperties**

> AMQP 0-9-1 协议预定义了一组 14 个与消息一起使用的属性。大多数属性很少使用，以下是常用属性：
>
> - Persistent：将消息标记为持久（值为true）或瞬态（任何其他值）。看看[第二个教程](https://www.rabbitmq.com/tutorials/tutorial-two-dotnet.html)。
> - DeliveryMode：熟悉协议的人可能会选择使用此属性而不是Persistent。他们控制着同样的事情。
> - ContentType：用于描述编码的 MIME 类型。例如，对于经常使用的 JSON 编码，最好将此属性设置为：application/json。
> - ReplyTo：通常用于命名回调队列。
> - CorrelationId：用于将 RPC 响应与请求相关联。

##### 相关ID

> 为每个 RPC 请求创建一个回调队列,这是非常低效的，但幸运的是有一个更好的方法——让我们为每个客户端创建一个回调队列。
>
> 在该队列中收到响应后，尚不清楚该响应属于哪个请求。这 就是使用 **CorrelationId** 属性的时候。我们将为每个请求设置一个唯一值。稍后，当我们在回调队列中收到一条消息时，我们将查看此属性，并基于此将响应与请求进行匹配。如果我们看到未知的 **CorrelationId** 值，我们可以安全地丢弃该消息——它不属于我们的请求。
>
> 为什么我们要忽略回调队列中的未知消息，而不是因为错误而失败？这是由于服务器端可能存在竞争条件。虽然不太可能，但 RPC 服务器可能会在向我们发送答案之后但在发送请求的确认消息之前死掉。如果发生这种情况，重新启动的 RPC 服务器将再次处理该请求。这就是为什么在客户端上我们必须优雅地处理重复响应，并且 RPC 理想情况下应该是幂等的。



### 交换器 （分组）

> 交换器必须确切地知道如何处理它收到的消息。
>
> 是否应该将其附加到特定队列？它应该附加到许多队列中吗？或者它应该被丢弃。
>
> 其规则由 **交换类型**  定义。

##### 交换类型

> - [direct](https://www.rabbitmq.com/tutorials/tutorial-four-dotnet.html): 让消息进入 **绑定键** 与 **消息的路由键** 完全匹配的队列
>
> - [topic](https://www.rabbitmq.com/tutorials/tutorial-five-dotnet.html):使用特定路由键发送的消息将被传递到与匹配绑定键绑定的所有队列
>
>   - *（star）可以只替换一个单词。
>   - \# (hash) 可以代替零个或多个单词。
>
>   > 主题交换功能强大，可以像其他交换一样运行。
>   >
>   > 当队列与 “ # ”（hash）绑定键绑定时 - 无论路由键如何，它将接收所有消息就像在 **fanout ** 交换中一样。
>   >
>   > 当绑定中不使用特殊字符 “ * ”（star）和“ # ”（hash）时，主题交换的行为就像 **direct ** 交换一样。
>
> - headers :
>
> - [fanout](https://www.rabbitmq.com/tutorials/tutorial-three-dotnet.html): 将收到的所有消息广播到它知道的所有队列

##### 声明交换器

```csharp
channel.ExchangeDeclare( "日志" , ExchangeType.Fanout);
```

##### 配置路由交换 （将队列分组）

> 在配置前需要声明交换器，因为禁止发布到不存在的交易所

```csharp
    channel.BasicPublish(exchange: "" , // 交换器名称
                         routingKey: "hello" , //路由键：目标队列
                         basicProperties: null , 
                         body: body);
```

> 第一个参数是交换器的名称。空字符串表示默认或*无名*交换：消息被路由到具有routingKey指定的名称的队列（如果存在）

> 如果没有队列绑定到交换器，消息将丢失，但这对我们来说没关系；如果没有消费者在监听，我们可以安全地丢弃消息。

##### 绑定队列交换

```csharp
channel.QueueBind(queue: queueName, // 队列名称
                  exchange: "logs" , // 交换器名称
                  routingKey: "" );// 绑定键
```

> 为避免与 **BasicPublish**  **路由键** 参数混淆，这里其称为 **绑定键**
>
> 绑定键的含义取决于交换类型

### 队列

##### 临时队列

```c#
// 声明一个队列 声明后不允许 使用不同参数重新定义现有队列
// 并且会向任何尝试这样做的程序返回错误
channel.QueueDeclare(queue: "hello",// 队列名称
    durable: false,// 标记是否将队列持久化 
    exclusive: false,
    autoDelete: false,
    arguments: null);
```

> 每当我们连接到 Rabbit 时，我们都需要一个新的空队列。为此，我们可以创建一个具有随机名称的队列，或者甚至更好 - 让服务器为我们选择一个随机队列名称。
>
> 其次，一旦我们断开消费者的连接，队列应该会被自动删除。
>
> 在 .NET 客户端中，当我们不向QueueDeclare()提供任何参数时， 会创建一个具有生成名称的非持久、独占、自动删除队列：
>
> ```csharp
> var queueName = channel.QueueDeclare().QueueName;
> ```



### 消息持久化

##### 将队列设置为持久化

```c#
// 声明一个队列 声明后不允许 使用不同参数重新定义现有队列
// 并且会向任何尝试这样做的程序返回错误
channel.QueueDeclare(queue: "hello",// 队列名称
    durable: true,// 标记是否将队列持久化 
    exclusive: false,
    autoDelete: false,
    arguments: null);
```



##### 将消息设置为持久化

```c#
var properties = channel.CreateBasicProperties();
properties.Persistent = true;// 标记是否将消息持久化

channel.BasicPublish(exchange: "",
    routingKey: "hello",
    basicProperties: properties,
    body: body);
```

##### 持久消息的确认延迟

> 路由到持久队列的持久消息的**basic.ack**将在将消息持久保存到磁盘后发送。RabbitMQ 消息存储在间隔（几百毫秒）后将消息分批保存到磁盘，以最小化 fsync(2) 调用的数量，或者当队列空闲时。
>
> 这意味着在恒定负载下， **basic.ack**的延迟可以达到几百毫秒。为了提高吞吐量，强烈建议应用程序异步处理确认（作为流）或发布批量消息并等待未完成的确认。用于此的确切 API 因客户端库而异。

##### 发布者确认模式异常情况

> 如果 RabbitMQ 节点在所述消息被写入磁盘之前发生故障，它可能会丢失持久性消息。
>
> 例如，考虑这种情况：
>
> 1. 客户端向一个持久化队列发布了一条持久化消息
> 2. 客户端使用队列中的消息（注意消息是持久的并且队列是持久的），但确认不是活动的，
> 3. 代理节点失败并重新启动，并且
> 4. 客户端重新连接并开始使用消息
>
> 此时，客户端可以合理地假设消息将被再次传递。情况并非如此：重新启动导致代理丢失了消息。为了保证持久性，客户端应该使用确认。如果发布者的频道处于确认模式，发布者将不会收到丢失消息的 **Ack**（因为消息尚未写入磁盘）。

##### 注意事项

> 将消息标记为持久性并不能完全保证消息不会丢失。虽然它告诉 RabbitMQ 将消息保存到磁盘，但是当 RabbitMQ 接受消息并且还没有保存它时，仍然有一个短暂的时间窗口。此外，RabbitMQ 并没有为每条消息做 fsync(2) -- 它可能只是被保存到缓存中，而不是真正写入磁盘。持久性保证并不强，但对于我们的简单任务队列来说已经足够了。
>
> 如果你需要更强的保证，那么你可以使用发布者确认。

### rabbitmqctl 命令行

##### 查看未确认的消息

> 可以使用rabbitmqctl 打印messages_unacknowledged字段：
>
> ```bash
> sudo rabbitmqctl list_queues name messages_ready messages_unacknowledged
> ```
>
> 
>
> 在 Windows 上，删除 sudo：
>
> ```bash
> rabbitmqctl.bat list_queues name messages_ready messages_unacknowledged
> ```

##### 查看交换器

> ```bash
> sudo rabbitmqctl list_exchanges
> ```
>
> 在这个列表中会有一些amq.*交换和默认（未命名）交换。这些是默认创建的

##### 列出交换器绑定

```bash
rabbitmqctl list_bindings
# => 列出绑定 ... 
# => 日志交换 amq.gen-JzTY20BRgKO-HjmUJj0wLg queue [] 
# => 日志交换 amq.gen-vso0PVvyiRIL2WoV3i48Yg queue [] 
# => ...完成。
```



### 关于 RPC 的说明

> 尽管RPC在计算中是一个相当常见的模式，但它经常被批评。当程序员不知道一个函数调用是本地的还是一个缓慢的RPC时，就会出现问题。诸如此类的混淆导致了系统的不可预测性，并为调试增加了不必要的复杂性。滥用RPC不但不能简化软件，反而会导致不可维护的意大利面条代码。

考虑到这一点，请考虑下面的建议。

- 确保清楚哪个函数调用是本地的，哪个是远程的。
- 记录您的系统。明确组件之间的依赖关系。
- 处理错误情况。RPC服务器长时间宕机时，客户端应该如何反应？

> 如有疑问，请避免使用 RPC。如果可以，您应该使用异步管道——而不是类似 RPC 的阻塞，结果被异步推送到下一个计算阶段。
