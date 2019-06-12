using System.Collections.Generic;
using UnityEngine;


namespace PhotonInMaze.Maze.Generator {

    internal class CellsToVisit {
        HashSet<CellToVisit> set;

        internal CellsToVisit() {
            this.set = new HashSet<CellToVisit>();
        }

        internal bool Add(CellToVisit ctv) {
            return this.set.Add(ctv);
        }


        internal bool Contains(CellToVisit ctv) {
            return set.Contains(ctv);
        }

        internal bool Remove(CellToVisit ctv) {
            return set.Remove(ctv);
        }

        internal CellToVisit FindRandom(int rows, int columns) {
            HashSet<CellToVisit>.Enumerator enumerator = set.GetEnumerator();
            Random.InitState(Random.Range(0, rows * columns * 11));
            int i = 0, random = Random.Range(0, set.Count);
            while(enumerator.MoveNext() && i < random) {
                i++;
            }
            return enumerator.Current;
        }

        internal bool IsNotEmpty() {
            return set.Count > 0;
        }
    }
}