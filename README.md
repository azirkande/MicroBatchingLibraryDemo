# MicroBatchingLibraryDemo

Micro-batching is a technique used in processing pipelines where individual tasks are grouped
together into small batches. This can improve throughput by reducing the number of requests made
to a downstream system. Your task is to implement a micro-batching library, with the following
requirements:
- It should expose a method which accepts a single Job and returns a JobResult
- It should process accepted Jobs in batches using a BatchProcessor
- The maximum batch size should be configurable
- The maximum time between calls made to BatchProcessor should be configurable
- It should expose a shutdown method which returns after all previously accepted Jobs are
processed. 



### Assumptions
- If there are not enough jobs to batch then Batch won't be created.
- I did not understand what shutdown method means, so I assume it as a method which gives details about messages processed so far.
- The maximum time between calls is used as latency to send next batch to BatchProcessor.

### Enhancements
- Use of IOC container libs. Right now used new to initialized objects
- Use of Logger. Use of Serilog to create structured log which can be further used to improve observability.
- Implement something further to handle failed tasks.

## Components:

All the components below are designed considering Single responsibility. 

### BatchingLibrary.Core :
This project holds the common stuff to be shared across other Libraries. Rightnow it contains dummy database modals. In the production it would be ORM model's entities.

### BatchingLibrary.Main: 
This is library can be consumed by API. Due to time constraints just created it as a Library, but ideally it should be consumed by an API and expose the methods

There is lot of scope to add new features in the SendJob and GetProcessedJob methods. 
- Something like addtional validation on the incoming payload(JSON). Specific JSON Schema validation.
- GetProcessedJob is just returning processed job limited by size. Future enhancements could be more filters like different staus, date range etc.

### BatchGenerator:
This component is responsible for geneating batches of incoming jobs. This lib should be used by a service running in the background and fetching new tasks to process.
What should happen to Failed Task is not in the scope.If there are no enough jobs to batch then Batch won't be created.

### BatchRunner:
This lib should be used by a service running in the background.It sends the batch at a time to BatchProcessor. It uses latency to decide if the Processor should be send a next batch or not. After the batch is processed it just updates batch and jobs statuses. 

### BatchingLib.Tests:
Covers tests of all the components involved in the project.
