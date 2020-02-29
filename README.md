# ConsoleResultPublisher for [Scientist.NET](https://github.com/scientistproject/Scientist.net)

Every young scientist knows that it's publish or perish! 

So to get you up and running quickly with [Scientist.NET](https://github.com/scientistproject/Scientist.net) here is a `ConsoleResultPublisher`

### Add the NuGet package

`Install-Package Scientist.Publishers.Console`

### Configure the publisher

You can use either .NET Core's Dependency Injection like this:


    //using GitHub;
    //using Scientist.Publishers.Console;
    
    public void ConfigureServices(IServiceCollection services)
    {
       //...omitted...
       
       services.AddTransient<IScientist, GitHub.Scientist>();
       services.AddTransient<IResultPublisher, ConsoleResultPublisher>();
       
       //alternatively you can write to the console asynchronously on a background thread like so
       //services.AddTransient<IResultPublisher>(provider => new FireAndForgetResultPublisher(new ConsoleResultPublisher()));
       
       //...omitted...
    }

Or simply configure it statically like this:

    //Either in Global.asax.cs or Program.cs
    GitHub.Scientist.ResultPublisher = new ConsoleResultPublisher();

Next, don your labcoat, strap on your goggles and run your experiment!

        [HttpGet("FizzBuzz")]
        public IEnumerable<string> FizzBuzz(int rangeStart = 1, int count = 100)
        {
            
            var input = Enumerable.Range(rangeStart, count);

            //Trying a new experimental formula for FizzBuzz.
            //Our FizzBuzzers take an IEnumerable<int> and return an IEnumerable<string>
            
            //Our experiment below is configured to return an IEnumerable<string>
            //And publish our results as a Dictionary<string, int> 
            //Summarising how many times Fizz, Buzz and FizzBuzz appear in our output.
            
            //We could also publish the raw data by simply configuring our experiment like
            //_scientist.Experiment<IEnumerable<string>>("experimental-fizz-buzz", ...);

            var output = _scientist.Experiment<IEnumerable<string>, Dictionary<string, int>>("experimental-fizz-buzz", experiment =>
            {
                experiment.Use(() => new DefaultFizzBuzz().ToFizzBuzz(input));
                experiment.Try("functional-fizz-buzz",() => new FunctionalFizzBuzz().ToFizzBuzz(input));
                
                experiment.Compare((control, candidate) => candidate.SequenceEqual(control)); 
                
                experiment.AddContext("rangeStart", rangeStart);
                experiment.AddContext("count", count);
                experiment.Clean(result =>
                {
                    result = result.ToList();
                    
                    var fizzBuzzStats = new Dictionary<string, int>
                    {
                        {"Fizz", result.Count(s => s.Equals("Fizz"))},
                        {"Buzz", result.Count(s => s.Equals("Buzz"))},
                        {"FizzBuzz", result.Count(s => s.Equals("FizzBuzz"))}
                    };

                    return fizzBuzzStats;
                });
            });
            
            return output;
        }

Experiments will be published to the console like so. No peer review needed!

    4f5c93b6-b946-4c90-b6c5-fb3a39d95323 experiment_name: experimental-fizz-buzz
    4f5c93b6-b946-4c90-b6c5-fb3a39d95323 results: {"matched":1,"mismatched":0,"ignored":0}
    4f5c93b6-b946-4c90-b6c5-fb3a39d95323 context: {"rangeStart":1,"count":100}
    4f5c93b6-b946-4c90-b6c5-fb3a39d95323 control_duration: 00:00:00.0004891
    4f5c93b6-b946-4c90-b6c5-fb3a39d95323 control_value: {"Fizz":27,"Buzz":14,"FizzBuzz":6}
    4f5c93b6-b946-4c90-b6c5-fb3a39d95323 candidate_name: functional-fizz-buzz
    4f5c93b6-b946-4c90-b6c5-fb3a39d95323 candidate_result: match
    4f5c93b6-b946-4c90-b6c5-fb3a39d95323 candidate_duration: 00:00:00.0012472
    4f5c93b6-b946-4c90-b6c5-fb3a39d95323 candidate_value: {"Fizz":27,"Buzz":14,"FizzBuzz":6}

