package main

import (
	"bufio"
	"fmt"
	"os"
	"sort"
	"strconv"
	"strings"
)

type Range struct {
	srcStart, destStart, length int64
}

type Seed struct {
	Number      int64
	Soil        int64
	Fertilizer  int64
	Water       int64
	Light       int64
	Temperature int64
	Humidity    int64
	Location    int64
}

type Map map[int64]int64

var seedToSoil []Map
var soilToFertilizer []Map
var fertilizerToWater []Map
var waterToLight []Map
var lightToTemperature []Map
var temperatureToHumidity []Map
var humidityToLocation []Map

func main() {
	file, err := os.Open(os.Args[1])
	if err != nil {
		fmt.Println("Error opening file:", err)
		return
	}
	defer file.Close()

	scanner := bufio.NewScanner(file)
	seeds, err := parseAlmanac(scanner)
	if err != nil {
		fmt.Println("Error reading lines:", err)
		return
	}

	// for _, seed := range seeds {
	// 	fmt.Printf("Seed %d:\n", seed.Number)
	// 	fmt.Printf("\tSoil: %d\n", seed.Soil)
	// 	fmt.Printf("\tFertilizer: %d\n", seed.Fertilizer)
	// 	fmt.Printf("\tWater: %d\n", seed.Water)
	// 	fmt.Printf("\tLight: %d\n", seed.Light)
	// 	fmt.Printf("\tTemperature: %d\n", seed.Temperature)
	// 	fmt.Printf("\tHumidity: %d\n", seed.Humidity)
	// 	fmt.Printf("\tLocation: %d\n", seed.Location)
	// }

	// Print the seed with the lowest location number
	lowestLocation := seeds[0].Location
	lowestLocationSeed := seeds[0]
	for _, seed := range seeds {
		if seed.Location < lowestLocation {
			lowestLocation = seed.Location
			lowestLocationSeed = seed
		}
	}
	fmt.Printf("Seed %d has lowest location: %d\n", lowestLocationSeed.Number, lowestLocationSeed.Location)
}

func parseAlmanac(scanner *bufio.Scanner) ([]Seed, error) {
	// Parse the seeds
	seeds, err := parseSeeds(scanner)
	if err != nil {
		return nil, err
	}
	// Print seeds
	// for _, m := range seeds {
	// 	fmt.Printf("Seed: %d\n", m.Number)
	// }

	// Initialize the slice of transformation maps
	maps := make([]Map, 7)

	// Populate the transformation maps
	for i := 0; i < len(maps); i++ {
		maps[i] = populateMap(scanner)
	}

	// print maps
	// printMaps(maps)

	// Apply the transformations
	for i := range seeds {
		number := seeds[i].Number
		for j, mapPtr := range maps {
			number = findDestination(number, mapPtr)
			switch j {
			case 0:
				seeds[i].Soil = number
			case 1:
				seeds[i].Fertilizer = number
			case 2:
				seeds[i].Water = number
			case 3:
				seeds[i].Light = number
			case 4:
				seeds[i].Temperature = number
			case 5:
				seeds[i].Humidity = number
			case 6:
				seeds[i].Location = number
			}
		}
	}

	return seeds, nil
}

func parseSeeds(scanner *bufio.Scanner) ([]Seed, error) {
	var seeds []Seed

	// Read the line that starts with "seeds:"
	scanner.Scan()
	line := scanner.Text()

	// Split the line into parts
	parts := strings.Split(line, " ")

	// Skip the first part ("seeds:")
	for _, part := range parts[1:] {
		// Convert the part to an int64
		number, err := strconv.ParseInt(part, 10, 64)
		if err != nil {
			return nil, err
		}

		// Create a Seed instance and add it to the slice
		seeds = append(seeds, Seed{Number: number})
	}
	// skip next empty line
	scanner.Scan()

	return seeds, nil
}

func findDestination(source int64, m Map) int64 {
	if destination, ok := m[source]; ok {
		return destination
	}
	// If no matching source is found, return the source number
	return source
}

func populateMap(scanner *bufio.Scanner) Map {
	m := make(Map)

	// Read the lines of the current block
	for scanner.Scan() {
		line := scanner.Text()

		// If the line is empty, break the loop
		if line == "" {
			break
		}

		// If the line ends with "map:", read the next line but don't break the loop
		if strings.HasSuffix(line, "map:") {
			continue
		}

		// Split the line into parts and convert the parts to integers
		parts := strings.Split(line, " ")
		destStart, _ := strconv.ParseInt(parts[0], 10, 64)
		srcStart, _ := strconv.ParseInt(parts[1], 10, 64)
		length, _ := strconv.ParseInt(parts[2], 10, 64)

		// Populate the map with source and destination numbers
		for i := int64(0); i < length; i++ {
			m[srcStart+i] = destStart + i
		}
	}

	return m
}

func printMaps(maps []Map) {
	// print maps
	for i, m := range maps {
		fmt.Printf("Map %d:\n", i+1)

		// Get the keys and sort them
		keys := make([]int, 0, len(m))
		for k := range m {
			keys = append(keys, int(k))
		}
		sort.Ints(keys)

		// Print the map entries in order of the sorted keys
		for _, k := range keys {
			fmt.Printf("\t%d -> %d\n", k, m[int64(k)])
		}
	}
}
