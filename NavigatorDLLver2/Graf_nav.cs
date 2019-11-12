using System;
using System.Collections.Generic;

namespace NavigationDLLver2
{
    public class Graf_nav
    {
        public string tree_file;
        public bool blocker;
        public bool block_suc;
        public bool reverse_A_breaker;
        private long index_intersection;
        public Dictionary<long, Path_nav> tree = new Dictionary<long, Path_nav>();
        /// <summary>
        /// Конструктор графа
        /// </summary>
        /// <param name="data">Данные</param>
        /// <param name="node_separator">Разделитель по вершинам</param>
        /// <param name="data_separator">Разделитель по данным вершин</param>
        /// <param name="massive_element_separator">Разделитель колекций в данных для вершины</param>
        public Graf_nav(string data, char node_separator = '!', char data_separator = '/', char massive_element_separator = ';')
        {
            reverse_A_breaker = false;
            blocker = false;
            block_suc = false;
            index_intersection = -1;
            string[] pars_data = data.Split(node_separator);
            for (int i = 0; i < pars_data.Length; i++)
            {
                long index = Convert.ToInt64(pars_data[i].Split(data_separator)[0]);
                Path_nav new_path = new Path_nav(pars_data[i], data_separator, massive_element_separator);
                tree.Add(index, new_path);
            }
        }
        public Graf_nav()
        {
            reverse_A_breaker = false;
            blocker = false;
            block_suc = false;
            index_intersection = -1;
        }
        public Path_nav this[long index]
        {
            get
            {
                return tree[index];
            }
        }
        /// <summary>
        /// Добавление вершины в граф
        /// </summary>
        /// <param name="index">Родительская вершина</param> 
        /// <param name="newNode">Новая вершина</param>
        public void AddNode(long index, Path_nav newNode)
        {
            tree.Add(index, newNode);
        }
        /// <summary>
        /// Дополняем колекцию доступных узлов
        /// </summary>
        /// <param name="out_data">Выходная колекция</param>
        /// <param name="index">Текущая вершина</param>
        /// <param name="root_index">Вершина из которой пришли</param>
        private void AddOutData(ref Dictionary<long, List<NavigationWays>> out_data, long index, long root_index, long end)
        {
            long[] broth_indexes = new long[tree[index].path.Keys.Count];
            tree[index].path.Keys.CopyTo(broth_indexes, 0);
            double path_crossed = 0;
            if (tree.ContainsKey(root_index))
                path_crossed = tree[root_index].temp_path_crossed;
            if (!out_data.ContainsKey(index))
            {
                out_data.Add(index, new List<NavigationWays>());
                foreach (long broth_ind in broth_indexes)
                {
                    if (broth_ind != root_index)
                    {
                        if (tree[broth_ind].temp_path_crossed == 0 ||
                            tree[broth_ind].temp_path_crossed > path_crossed + tree[index][broth_ind])
                        {
                            out_data[index].Add(
                                new NavigationWays(broth_ind, path_crossed + tree[index][broth_ind],
                                Path_nav.calculationCost(tree[broth_ind].param, tree[end].param)));
                        }
                    }
                }
            }
        }
        /// <summary>
        /// Минимальный F(x) из колекции вершин
        /// </summary>
        /// <param name="array">Колекция вершин</param>
        /// <returns>Следующий узел для выбора</returns>
        private static NavigationWays GetMin(Dictionary<long, List<NavigationWays>> array, out long index)
        {
            index = -1;
            NavigationWays return_value = new NavigationWays();
            double min = 40076001;
            foreach (var parent_item in array)
                foreach (var item in parent_item.Value)
                    if (item.cost < min)
                    {
                        index = parent_item.Key;
                        min = item.cost;
                        return_value = item;
                    }
            return return_value;
        }
        /// <summary>
        /// Рекурсивная функция для получения пути
        /// </summary>
        /// <param name="array">Колекция просмотренных вершин</param>
        /// <param name="prev_ind">Вершина к которой нужно проложить путь</param>
        /// <param name="way">Колекция с индексами вершин, которые будут проидены</param>
        private static void getNextIndex_A(Dictionary<long, List<NavigationWays>> array, long prev_ind, ref List<long> way)
        {
            long temp_ind = -1;
            double min = 40076001;
            foreach (var item_list in array)
                foreach (var item in item_list.Value)
                    if (item.destination_index == prev_ind && item.real_cost < min)
                        if (!way.Contains(item_list.Key))
                        {
                            min = item.real_cost;
                            temp_ind = item_list.Key;
                        }
            if (temp_ind != -1)
            {
                way.Add(temp_ind);
                getNextIndex_A(array, temp_ind, ref way);
            }
        }
        /// <summary>
        /// Нахождение пути по алгоритму А*
        /// </summary>
        /// <param name="tree">Граф</param>
        /// <param name="start">Начальная вершина</param>
        /// <param name="end">Конечная вершина</param>
        /// <param name="distans">Пройденный путь</param>
        /// <returns>Колекция вершин, что нужно пройти</returns>
        public List<long> getWay_A(long start, long end, out double distans, ref Dictionary<long, List<NavigationWays>> Not_Look, ref Dictionary<long, List<NavigationWays>> Look, bool reverse = false)
        {
            List<long> way = new List<long>();
            distans = 0;
            char new_flag = 'f';
            char other_flag = 's';
            if (reverse)
            {
                new_flag = 's';
                other_flag = 'f';
            }
            Not_Look = new Dictionary<long, List<NavigationWays>>();
            Look = new Dictionary<long, List<NavigationWays>>();
            AddOutData(ref Not_Look, start, -1, end);
            long index;
            NavigationWays temp = GetMin(Not_Look, out index);
            tree[temp.destination_index].temp_path_crossed = tree[start][temp.destination_index];
            /*if (distans < tree[index].temp_path_crossed)
                NotLook*/
            if (!Look.ContainsKey(index))
                Look.Add(index, new List<NavigationWays>());
            Look[index].Add(temp);
            Not_Look[index].Remove(temp);
            long root_index;
            while (index != end)
            {
                if (reverse_A_breaker)
                {
                    return way;
                }
                if (!blocker)
                {
                    if (index_intersection == -1)
                    {
                        foreach(var item in Look[index])
                        { 
                            AddOutData(ref Not_Look, item.destination_index, index, end);
                        }
                        if (Not_Look.Count == 0)
                        {
                            reverse_A_breaker = true;
                            return way;
                        }
                        root_index = index;
                        temp = GetMin(Not_Look, out index);
                        if (!Look.ContainsKey(index))
                            Look.Add(index, new List<NavigationWays>());
                        Look[index].Add(temp);
                        Not_Look[index].Remove(temp);
                        if (Not_Look[index].Count == 0)
                            Not_Look.Remove(index);
                        //distans += tree[root_index][index];
                        tree[temp.destination_index].temp_path_crossed = tree[index].temp_path_crossed + tree[index][temp.destination_index];
                        if (tree[index].flag == other_flag)
                        {
                            index_intersection = index;
                            //distans = tree[root_index].temp_path_crossed+tree[root_index][index];
                            break;
                        }
                        tree[index].flag = new_flag;
                    }
                    else
                        break;
                }
                else
                    block_suc = true;
            }
            if (index_intersection == -1)
            {
                reverse_A_breaker = true;
                way.Add(end);
            }
            else
                way.Add(index_intersection);
            distans = tree[way[0]].temp_path_crossed;
            getNextIndex_A(Look, way[0], ref way);
            way.Reverse();
            return way;
        }

    }
    public struct NavigationWays : IComparable<NavigationWays>
    {
        public long destination_index;//Вершина назначения
        public double cost;//F(x)
        public double real_cost;//f(x)
        public NavigationWays(long ind, double r_c, double heuristic)
        {
            destination_index = ind;
            real_cost = r_c;
            cost = r_c + heuristic;
        }
        public string ToString(long index_root)
        {
            return "Из вершины " + index_root + " в вершину " + destination_index + " " + cost + " m";
        }
        public int CompareTo(NavigationWays obj)
        {
            if (this.cost > obj.cost)
                return 1;
            if (this.cost < obj.cost)
                return -1;
            else
                return 0;
        }
    }
}

