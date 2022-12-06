List<string> packetMarkerList = new List<string>();
List<string> messageMarketList = new List<string>();
bool startOfPacketFound = false;
int counter = 0;
using (var sr = new StreamReader(".\\..\\..\\..\\input.txt"))
{
    while (!sr.EndOfStream)
    {
        counter++;
        var current = ((char)sr.Read()).ToString();

        if (!startOfPacketFound) {
            packetMarkerList.Add(current);

            if (packetMarkerList.Count() == 4)
            {
                if (packetMarkerList.Distinct().Count() == 4)
                {
                    // Part 1
                    Console.WriteLine(counter);
                    startOfPacketFound = true;
                }
                else
                {
                    packetMarkerList.RemoveAt(0);
                }
            }
        }

        messageMarketList.Add(current);
        if (messageMarketList.Count() == 14)
        {
            if (messageMarketList.Distinct().Count() == 14)
            {
                // Part 2
                Console.WriteLine(counter);
                break;
            }
            else
            {
                messageMarketList.RemoveAt(0);
            }
        }
    }
}