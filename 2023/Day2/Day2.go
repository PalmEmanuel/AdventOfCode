package main

// Game 1: 3 blue, 4 red; 1 red, 2 green, 6 blue; 2 green
// Game 2: 1 blue, 2 green; 3 green, 4 blue, 1 red; 1 green, 1 blue
// Game 3: 8 green, 6 blue, 20 red; 5 blue, 4 red, 13 green; 5 green, 1 red
// Game 4: 1 green, 3 red, 6 blue; 3 green, 6 red; 3 green, 15 blue, 14 red
// Game 5: 6 red, 1 blue, 3 green; 2 blue, 1 red, 2 green

import (
	"bufio"
	"fmt"
	"os"
	"regexp"
	"strconv"
	"strings"
)

const redLimit = 12
const greenLimit = 13
const blueLimit = 14

type Game struct {
	ID   string
	Sets []map[string]int
}

func main() {
	// Open file based on command line argument, DataInput.txt or ExampleInput.txt
	file, _ := os.Open(os.Args[1])
	defer file.Close()

	scanner := bufio.NewScanner(file)
	gameIDAndSetsRegex := regexp.MustCompile(`Game (\d+): (.*)`)
	colorAndNumberRegex := regexp.MustCompile(`(\d+) (\w+)`)

	var games []Game

	for scanner.Scan() {
		line := scanner.Text()
		matches := gameIDAndSetsRegex.FindStringSubmatch(line)

		if len(matches) == 3 {
			gameID := matches[1]
			setsStr := matches[2]
			sets := strings.Split(setsStr, ";")

			var gameSets []map[string]int

			for _, set := range sets {
				set = strings.TrimSpace(set)
				colorsAndNumbers := colorAndNumberRegex.FindAllStringSubmatch(set, -1)

				colorNumberMap := make(map[string]int)

				for _, cn := range colorsAndNumbers {
					color := cn[2]
					number := cn[1]
					colorNumberMap[color], _ = strconv.Atoi(number)
				}

				gameSets = append(gameSets, colorNumberMap)
			}

			games = append(games, Game{ID: gameID, Sets: gameSets})
		}
	}

	part1Sum := 0
	part2Sum := 0
	for _, game := range games {
		redMax, greenMax, blueMax := 0, 0, 0
		for _, set := range game.Sets {
			if set["red"] > redMax {
				redMax = set["red"]
			}
			if set["green"] > greenMax {
				greenMax = set["green"]
			}
			if set["blue"] > blueMax {
				blueMax = set["blue"]
			}
		}

		gameID, _ := strconv.Atoi(game.ID)

		redString := fmt.Sprintf("%d", redMax)
		greenString := fmt.Sprintf("%d", greenMax)
		blueString := fmt.Sprintf("%d", blueMax)
		addToSumPart1 := true
		if redMax > redLimit {
			redString = fmt.Sprintf("\033[31m%s\033[0m", redString)
			addToSumPart1 = false
		}
		if greenMax > greenLimit {
			greenString = fmt.Sprintf("\033[31m%s\033[0m", greenString)
			addToSumPart1 = false
		}
		if blueMax > blueLimit {
			blueString = fmt.Sprintf("\033[31m%s\033[0m", blueString)
			addToSumPart1 = false
		}

		fmt.Printf("Game: %d - Red: %s, Green: %s, Blue: %s\n", gameID, redString, greenString, blueString)

		if addToSumPart1 {
			part1Sum += gameID
		}
		part2Sum += redMax * greenMax * blueMax
	}

	fmt.Printf("Answer: %d\n", part1Sum)
	fmt.Printf("Answer: %d\n", part2Sum)
}
