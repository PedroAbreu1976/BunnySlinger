using BunnySlinger;


namespace ConsoleApp1
{
    public class MyCuteBunny :IBunny
    {
        public string Name { get; set; }
    }

    public class MyBunnyCatcher : IBunnyCatcher<MyCuteBunny> {

        public static int _count = 0;
        public int id;
        
        public MyBunnyCatcher() {
            _count++;
            id = _count;
        }

        public Task<bool> CatchBunnyAsync(MyCuteBunny bunny) {
		    Console.WriteLine($"Received  Handler 1 [{id}]: {bunny.Name}");
		    return Task.FromResult(true);
	    }
    }

    public class MyOtherBunnyCatcher : IBunnyCatcher<MyCuteBunny>
    {
        public Task<bool> CatchBunnyAsync(MyCuteBunny bunny)
        {
            Console.WriteLine($"Received Handler 2: {bunny.Name}");
            return Task.FromResult(true);
        }
    }
}
