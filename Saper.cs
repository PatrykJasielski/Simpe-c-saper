using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Linq;
using System.Net;
using System.Net.Mime;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Saper
{
    internal class Program
    {
        public class Cords
        {
            public int x; public int y;

            public Cords(int x, int y)
            {
                this.x = x;
                this.y = y;
            }

        }
        public static int get_number(int a, int b)
        {
            int n;
            Console.WriteLine("Podaj liczbe: ");
            n = Convert.ToInt32(Console.ReadLine());
            return n;
        }
        public static List<Cords> generate_mines(int r, int c, int number_of_mines)
        {
            Random rnd = new Random();

            List<Cords> cords_list = new List<Cords>();

            for (int i = 0; i < number_of_mines; i++)
            {
                int k = rnd.Next(r);
                int v = rnd.Next(c);
                if (search_for_duplicates(number_of_mines, cords_list, k, v))
                    cords_list.Add(new Cords(k, v));
            }
            return cords_list;
        }
        public static bool search_for_duplicates(int number_of_mines, List<Cords> cords_list, int k, int v)
        {
            while (cords_list.Count == number_of_mines)
            {
                for (int i = 0; i < cords_list.Count; i++)
                {
                    if ((cords_list[i].x == k && cords_list[i].y == v) || (cords_list[i].x == v && cords_list[i].y == k))
                        return false;
                }
            }
            return true;
        }
        public static string number_of_neighboring_mines(List<Cords> mines, Cords square)
        {
            int count = 0;
            int i = square.x;
            int j = square.y;
            List<Cords> list_of_niegh = new List<Cords>();
            list_of_niegh.Add(new Cords(i - 1, j - 1 ));
            list_of_niegh.Add(new Cords(i - 1, j));
            list_of_niegh.Add(new Cords(i - 1, j + 1));
            list_of_niegh.Add(new Cords(i, j - 1));
            list_of_niegh.Add(new Cords(i, j + 1));
            list_of_niegh.Add(new Cords(i + 1, j - 1));
            list_of_niegh.Add(new Cords(i + 1, j));
            list_of_niegh.Add(new Cords(i + 1, j + 1));

            for (int k = 0; k < list_of_niegh.Count; k++)
            {
                if (search_for_mines(list_of_niegh[k].x, list_of_niegh[k].y, mines))
                    count++;
            }
            return Convert.ToString(" " + count + " ");
        }
        public static string[,] create_board(int r, int c, List<Cords> mines)
        {
            string[,] board = new string[r, c];
            for (int i = 0; i  < r; i++)
            {
                for (int j = 0; j < c; j++)
                {
                    if (search_for_mines(i, j, mines))
                    {
                        board[i, j] = " * ";
                    }
                    else
                    {
                        Cords square = new Cords(i, j);

                        board[i, j] = number_of_neighboring_mines(mines, square);
                    } 
                }
            }
            return board;
        }
        public static bool search_for_mines(int i, int j, List<Cords> mines)
        {
            for (int k = 0; k < mines.Count; k++)
            {
                if ((mines[k].x == i && mines[k].y == j))
                {
                    return true;
                }
            }
            return false;
        }
        public static void Reveal_fields(Cords position, string[,] board, int r, int c, List<Cords> squares)
        {
            int i = position.x;
            int j = position.y;

            if (squares.Any(xy => xy.x == i && xy.y == j) || i < 0 || i >= r || j < 0 || j >= c)
                return;
            squares.Add(new Cords(i, j));

            if (board[i, j] != " 0 ")
                return;

            List<Cords> neighbors = new List<Cords>();
            neighbors.Add(new Cords(i - 1, j - 1));
            neighbors.Add(new Cords(i - 1, j));
            neighbors.Add(new Cords(i - 1, j + 1));
            neighbors.Add(new Cords(i, j - 1));
            neighbors.Add(new Cords(i, j + 1));
            neighbors.Add(new Cords(i + 1, j - 1));
            neighbors.Add(new Cords(i + 1, j));
            neighbors.Add(new Cords(i + 1, j + 1));

            foreach (Cords p in neighbors)
            {
                Reveal_fields(p, board, r, c, squares);
            }
        }


        public static void print_board(string[,] board, int r, int c, List<Cords> squares, bool show_all = false)
        {
            Console.Write("   ");
            for (int i =  0; i < r; i++)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(" " + i + " ");
            }
            Console.WriteLine();
            for (int i = 0; i < r; i++)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
                Console.Write(" " + i + " ");
                Console.ResetColor();
                for (int j = 0; j < c; j++)
                {
                    if (show_all || squares.Any(new_xy => new_xy.x == i && new_xy.y == j))
                        Console.Write(board[i, j]);
                    else 
                    {
                        Console.ForegroundColor = ConsoleColor.Green; 
                        Console.Write(" # "); 
                        Console.ResetColor();
                    }
                    
                }
                Console.WriteLine();
            }
        }
        static void Main(string[] args)
        {
            int r = 10, c = 10, number_of_mines = 10;
            List<Cords> mines = new List<Cords> ();
            mines = generate_mines(r, c, number_of_mines);
            string[,] board = new string[r, c];
            board = create_board(r, c, mines);
            List<Cords> squares = new List<Cords> ();
            List<Cords> position = new List<Cords>();

            while (squares.Count < r * c - number_of_mines)
            {
                print_board(board, r, c, squares);
                Console.WriteLine("Podaj numer wierszu: ");
                int i = get_number(0, c);
                Console.WriteLine("Podaj numer kolumny: ");
                int j = get_number(0, c);
                if (mines.Any(new_xy => new_xy.x == i && new_xy.y == j))
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    Console.WriteLine("GAME OVER!");
                    Console.ResetColor();
                    print_board(board, r, c, squares, true);
                    Console.ReadLine();
                    break;
                }
                Cords pos = new Cords (i, j);
                Reveal_fields(pos, board, r, c, squares);
            }
        }
    }
}
