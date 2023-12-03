package main

import (
	"bufio"
	"fmt"
	"log"
	"os"
	"strconv"
	"unicode"
)

type Number struct {
	Value  string
	Valid  bool
	Coords [][2]int
}

func main() {
	grid, _ := readLines(os.Args[1])

	var numbers []Number

	for i := range grid {
		j := 0
		for j < len(grid[i]) {
			if unicode.IsDigit(rune(grid[i][j])) {
				start := j
				for j < len(grid[i]) && unicode.IsDigit(rune(grid[i][j])) {
					j++
				}
				num := grid[i][start:j]
				valid := false
				coords := make([][2]int, j-start)
				for k := start; k < j; k++ {
					coords[k-start] = [2]int{i, k}
					for di := -1; di <= 1; di++ {
						for dj := -1; dj <= 1; dj++ {
							ni, nj := i+di, k+dj
							if ni >= 0 && ni < len(grid) && nj >= 0 && nj < len(grid[i]) && isSymbol(rune(grid[ni][nj])) {
								valid = true
								break
							}
						}
						if valid {
							break
						}
					}
					if valid {
						break
					}
				}
				numbers = append(numbers, Number{Value: num, Valid: valid, Coords: coords})
			} else {
				j++
			}
		}
	}

	sum1 := 0
	for _, number := range numbers {
		if number.Valid {
			num, _ := strconv.Atoi(number.Value)
			sum1 += num
		}
	}

	printGrid(grid, numbers)
	fmt.Printf("Part 1: %d\n", sum1)

	sum2 := 0
	for i := range grid {
		for j := range grid[i] {
			if grid[i][j] == '*' {
				adjacentNumbers := getAdjacentNumbers(grid, i, j, numbers)
				if len(adjacentNumbers) == 2 {
					var nums []Number
					for _, num := range adjacentNumbers {
						nums = append(nums, num)
					}
					num1, err1 := strconv.Atoi(nums[0].Value)
					if err1 != nil {
						log.Fatalf("Failed to parse %s as integer: %v", nums[0].Value, err1)
					}
					num2, err2 := strconv.Atoi(nums[1].Value)
					if err2 != nil {
						log.Fatalf("Failed to parse %s as integer: %v", nums[1].Value, err2)
					}
					sum2 += num1 * num2
				}
			}
		}
	}
	fmt.Printf("Part 2: %d\n", sum2)
}

func printGrid(grid []string, numbers []Number) {
	index := 0
	for i, line := range grid {
		for j := 0; j < len(line); {
			if unicode.IsDigit(rune(line[j])) {
				start := j
				for j < len(line) && unicode.IsDigit(rune(line[j])) {
					j++
				}
				num := line[start:j]
				if num == numbers[index].Value {
					if numbers[index].Valid {
						fmt.Printf("\033[32m%s\033[0m", num) // print number in green
					} else {
						fmt.Print(num)
					}
					index++
				}
			} else if line[j] == '*' {
				adjacentNumbers := getAdjacentNumbers(grid, i, j, numbers)
				if len(adjacentNumbers) == 2 {
					fmt.Printf("\033[33m%s\033[0m", string(line[j])) // print * in yellow
				} else {
					fmt.Print(string(line[j]))
				}
				j++
			} else {
				fmt.Print(string(line[j]))
				j++
			}
		}
		fmt.Println()
	}
}

func getAdjacentNumbers(grid []string, i int, j int, numbers []Number) map[string]Number {
	adjacentNumbers := make(map[string]Number)
	for di := -1; di <= 1; di++ {
		for dj := -1; dj <= 1; dj++ {
			ni, nj := i+di, j+dj
			if ni >= 0 && ni < len(grid) && nj >= 0 && nj < len(grid[i]) {
				for _, number := range numbers {
					if number.Valid {
						for _, coord := range number.Coords {
							if coord[0] == ni && coord[1] == nj {
								adjacentNumbers[number.Value] = number
								break
							}
						}
					}
				}
			}
		}
	}
	return adjacentNumbers
}

func isSymbol(c rune) bool {
	ascii := int(c)
	if ascii == 46 {
		return false
	}
	return (ascii >= 33 && ascii <= 47) || (ascii >= 58 && ascii <= 64) || (ascii >= 91 && ascii <= 96) || (ascii >= 123 && ascii <= 126)
}

func readLines(path string) ([]string, error) {
	file, _ := os.Open(path)
	defer file.Close()

	var lines []string
	scanner := bufio.NewScanner(file)
	for scanner.Scan() {
		lines = append(lines, scanner.Text()) // append each line to the lines slice
	}
	return lines, scanner.Err()
}
