using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;


namespace Projects
{
    class Program
    {
        static int last_day = 0;
        static string path = @"put\";
        static List<contributer> my_contributers = new List<contributer>();
        static List<project> my_projects = new List<project>();
        static public List<string> filenamelist = new List<string>();
        static public List<project_timeline> proj_timeline = new List<project_timeline>();
        static int time_passing = 0;
        static void Main(string[] args)
        {
            // 1 same skill level
            // 2 higher skill level
            // 3 release if project not picked up
            // 4 upgrade skill if equal
            // 5 go through projects that did not have popele first time around (executed = false) (till time runs out => if not expired) if time loop, leave
            // 6 // mentor, check if we already have mentor (if no skill, means zero)
            // 7 loop through best by?
            //  //// mentor, if mentor not already there, next person should be it

            // un comment each file to run it
            // 1- 0
            // 2- 30
            // 3- 30
            // 4- 
            // 5- 33
            // 6- 33
            // 7- 33
            // filenamelist.Add("a_an_example.in.txt");
            // 1- 2316
            // 2- 92746
            // 3- 188953
            // 4- WA
            // 5- 
            // 6- 400666
            // 7- 400666
            // filenamelist.Add("b_better_start_small.in.txt");
            // 1- 0
            // 2- 0
            // 3- 0
            // 4- WA
            // 5- 
            // 6- 0
            // filenamelist.Add("c_collaboration.in.txt");
            // 1- 5863
            // 2- 8807
            // 3- 53197
            // 4- 53197
            // 5- 
            // 6- 68303
            // 7- 68303
            // filenamelist.Add("d_dense_schedule.in.txt");
            // 1- 109465
            // 2- 356986
            // 3- 720477
            // 4- 720477
            // 5- 720477
            // 6- 723,201
            // 7- 1,634,875
            // filenamelist.Add("e_exceptional_skills.in.txt");
            // 1- 186306
            // 2- 120328
            // 3- 188730
            // 4- WA
            // 5- 
            // 6- 37,158
            // 7- 214
            filenamelist.Add("f_find_great_mentors.in.txt");
            // 1- total = 303,950 
            // 2- total = 578,897
            // 3- total = 1,151,387
            // 6- 1,192,203
            // 7- 2.1

            foreach (var myfilename in filenamelist)
            {
                Console.WriteLine("- Start Reading - " + myfilename);
                ReadMyInput(path + myfilename);

                output myoutput = new output();
                myoutput = solve();
                WriteOutput(myoutput, path + myfilename);
            }
        }
        // earlyest proj be done
        // assigned bool for person
        static output solve()
        {
            output my_output = new output();
            // put projest with same timeline together
            var objs = (from proj in my_projects
                        select new
                        {
                            list_of_ids = my_projects.Where(p => p.best_before_day == proj.best_before_day && p.duration_to_complete_day == proj.duration_to_complete_day).Select(p => p.proj_id).ToList(),
                            proj.best_before_day,
                            proj.duration_to_complete_day
                        }).Distinct().ToList();

            //https://stackoverflow.com/questions/7325278/group-by-in-linq
            // var best_by_list = my_projects.GroupBy(n => n.best_before_day)
            // .Select(r => new { best_before_day = r.Key, list_of_ids = r.ToList() })
            // .ToList();
            bool time_loop = false;
            int prev_time_passed = -1;
            int i = 0;
            do
            {
                Console.WriteLine(i + "th loop ---- " + i);
                i += 1;
                if (time_passing == prev_time_passed)
                    time_loop = true;
                else
                {
                    prev_time_passed = time_passing;
                }
                // duration to compelete
                // var duration_list = my_projects.Where(p => p.executed == false).GroupBy(n => n.duration_to_complete_day)
                // .Select(r => new { duration_to_complete_day = r.Key, list_of_proj = r.ToList() }).ToList();
                // best by
                var duration_list = my_projects.Where(p => p.executed == false).GroupBy(n => n.best_before_day)
                .Select(r => new { best_before_day = r.Key, list_of_proj = r.ToList() }).ToList();
                
                foreach (var same_length_proj in duration_list)
                {
                    foreach (var proj in same_length_proj.list_of_proj)
                    {
                        if (proj.project_name == "p22581")
                            Console.WriteLine("p22581");
                        op_2 myprojinfo = new op_2();
                        myprojinfo.project_name = proj.project_name;
                        myprojinfo.contributer_name_list = new List<string>(new string[proj.project_skills.Count()]).ToList();
                        List<skill> skills_need_mentor = new List<skill>();

                        foreach (var p_skill in proj.project_skills)
                        {
                            string person_name = "";
                            var needs_mentor = false;
                            //Assign Person

                            // exaxt same skill level, can upgrade
                            var selected_person = my_contributers.Where(v => v.is_available == true &&
                            v.conributer_skills.Where(p => p.skill_name == p_skill.skill_name && p.skill_level == p_skill.skill_level).Count() > 0).FirstOrDefault();
                            if (selected_person != null)
                            {
                                // can_upgrade cuz same skill level
                                my_contributers = my_contributers.Select(n => { if (n.contributer_name == selected_person.contributer_name) { n.can_upgrade = true; n.skill_to_upgrade = p_skill; } return n; }).ToList();

                            }
                            // if lower, then mentor is needed, aslo can upgrade
                            if (selected_person == null)
                            {
                                // if person has skill but it is -1
                                selected_person = my_contributers.Where(v => v.is_available == true &&
                                // testing count ==1, it used to be >0
                                v.conributer_skills.Where(p => (p.skill_name == p_skill.skill_name && p.skill_level == p_skill.skill_level - 1)).Count() == 1).FirstOrDefault();
                                // if person desn't have skill, which means level 0
                                needs_mentor = true;
                                // if does not have skill means zero, add skill to thier list with val 0, only when the needed skill is at level 1
                                if (p_skill.skill_level == 1 && selected_person == null)
                                {
                                    // get the first available person to help ^^
                                    // test if better to get person with little skill? only 1 skill, &&v.n_contributer_skills==1
                                    // person should not have skill, then skill is added

                                    selected_person = my_contributers.Where(v => v.is_available == true && v.conributer_skills.Where(c => c.skill_name == p_skill.skill_name && c.skill_level >= p_skill.skill_level).Count() == 0).FirstOrDefault();
                                    if (selected_person != null)
                                    {
                                        // if (selected_person.contributer_name == "c689" && p_skill.skill_name == "s44")
                                        //     Console.WriteLine(proj.project_name + " - " + (selected_person.contributer_name));

                                        my_contributers = my_contributers.Select(n =>
                                        {
                                            if (n.contributer_name == selected_person.contributer_name)
                                            {
                                                n.conributer_skills.Add(new skill(p_skill.position_id, p_skill.skill_name, 0, false));
                                                n.n_contributer_skills = n.conributer_skills.Count();
                                            }
                                            return n;
                                        }).ToList();
                                    }
                                }
                                if (selected_person != null)
                                {
                                    // check if we already have mentor
                                    // mentor can not be themselves
                                    var has_mentor = my_contributers.Where(p => p.contributer_name != selected_person.contributer_name &&
                                     myprojinfo.contributer_name_list.Contains(p.contributer_name) &&
                                    p.conributer_skills.Where(n => n.skill_name == p_skill.skill_name && n.skill_level >= p_skill.skill_level).Count() > 0).Count() > 0;
                                    //if no mentor, then person not selected
                                    if (!has_mentor)
                                        selected_person = null;
                                    else
                                    {
                                        my_contributers = my_contributers.Select(n =>
                                        {
                                            if (n.contributer_name == selected_person.contributer_name)
                                            {
                                                n.skill_to_upgrade = p_skill;
                                                n.can_upgrade = true;
                                            }
                                            return n;
                                        }).ToList();

                                        // if (selected_person.contributer_name == "c689" && p_skill.skill_name == "s44")
                                        //     Console.WriteLine(proj.project_name + " - " + (selected_person.contributer_name));
                                        // Console.WriteLine("Got a Mentor for " + p_skill.skill_name);
                                    }
                                }
                            }

                            // if above not found, get people with higher skill (can't upgrade)
                            if (selected_person == null)
                            {
                                selected_person = my_contributers.Where(v => v.is_available == true &&
                                v.conributer_skills.Where(p => p.skill_name == p_skill.skill_name && p.skill_level > p_skill.skill_level).Count() > 0).FirstOrDefault();
                                // set can upgrade to false in case of error
                                if (selected_person != null)
                                {
                                    my_contributers = my_contributers.Select(n =>
                                 {
                                     if (n.contributer_name == selected_person.contributer_name)
                                     {
                                         n.skill_to_upgrade = null;
                                         n.can_upgrade = false;
                                     }
                                     return n;
                                 }).ToList();
                                }
                            }
                            if (selected_person != null)
                            {
                                my_contributers = my_contributers.Select(n => { if (n.contributer_name == selected_person.contributer_name) { n.is_available = false; } return n; }).ToList();
                                proj.project_skills = proj.project_skills.Select(n => { if (n.skill_name == p_skill.skill_name && n.skill_level >= p_skill.skill_level) { n.is_taken = true; } return n; }).ToList();
                                // how to say is same level or took mentor? like can upgrade?
                                person_name = selected_person.contributer_name;

                                // if (person_name == "c689" && p_skill.skill_name == "s44")
                                //     Console.WriteLine(proj.project_name + " - " + (selected_person.contributer_name));
                            }
                            else
                            {
                                break;
                            }
                            myprojinfo.contributer_name_list[p_skill.position_id] = person_name;

                        }
                        // if all skills filled, ++?
                        if (proj.n_skills == myprojinfo.contributer_name_list.Where(p => p != "" && p != null).Count())
                        {
                            my_output.project_info.Add(myprojinfo);
                            time_passing += proj.duration_to_complete_day;
                            Console.WriteLine(time_passing + " day passed out of " + last_day + " total");
                            // if (641 <= time_passing && time_passing <= 659)
                            //     Console.WriteLine(" total");
                            // set proj as executed
                            my_projects = my_projects.Select(n => { if (n.project_name == myprojinfo.project_name) { n.executed = true; } return n; }).ToList();

                            // updgare skills of equal skilled ppl
                            my_contributers = my_contributers.Select(n =>
                            {
                                if (myprojinfo.contributer_name_list.Contains(n.contributer_name) && n.can_upgrade)//&& n.conributer_skills.Contains(n.skill_to_upgrade)
                                {
                                    n.conributer_skills = n.conributer_skills.Select(p =>
                                    {
                                        // can't say equal cuz taken val is not same
                                        if (p.skill_name == n.skill_to_upgrade.skill_name && p.position_id == n.skill_to_upgrade.position_id)
                                        {
                                            p.skill_level = p.skill_level + 1;
                                        }
                                        return p;
                                    }).ToList();
                                }
                                return n;
                            }).ToList();
                            // after skill upgraded, set all to false in case of error                            
                            my_contributers = my_contributers.Select(n =>
                            {
                                if (myprojinfo.contributer_name_list.Contains(n.contributer_name))
                                {
                                    n.skill_to_upgrade = null;
                                    n.can_upgrade = false;
                                }
                                return n;
                            }).ToList();
                        }
                        else
                        {
                            // release if proj didn't happen  
                            my_contributers = my_contributers.Select(p => { if (myprojinfo.contributer_name_list.Contains(p.contributer_name)) { p.is_available = true; } return p; }).ToList();

                        }
                    }
                    // release all cuz time passed
                    my_contributers = my_contributers.Select(n => { { n.is_available = true; } return n; }).ToList();
                }

            } while (time_passing <= last_day && time_loop == false);

            my_output.n_execeuted_projects = my_output.project_info.Count();
            return my_output;
        }
        static void WriteOutput(output obj, string full_path)
        {
            string createText = obj.n_execeuted_projects.ToString();

            foreach (var item in obj.project_info)
            {
                if (item.contributer_name_list.Count() != 0)
                {
                    createText += "\n" + item.project_name + "\n";
                    foreach (var contributers in item.contributer_name_list)
                    {
                        createText += contributers + " ";
                    }
                }
            }
            Console.WriteLine("n_execeuted_projects " + obj.n_execeuted_projects.ToString());

            File.WriteAllText(full_path + "_output.txt", createText);
        }
        static void ReadMyInput(string full_path)
        {
            var MyInputFile = File.ReadAllLines(full_path);

            var firstLine = MyInputFile[0].Split(' ');
            var n_of_contributer = int.Parse(firstLine[0]);
            var n_of_project = int.Parse(firstLine[1]);
            int lines_till_proj = 0;
            // get c
            for (int i = 1; i <= n_of_contributer; i++)
            {
                var currentline = MyInputFile[i].Split(' ');
                contributer my_contributer = new contributer();
                my_contributer.contributer_name = currentline[0];
                my_contributer.n_contributer_skills = int.Parse(currentline[1]);

                for (int j = i + 1; j <= i + my_contributer.n_contributer_skills; j++)
                {
                    var skillline = MyInputFile[j].Split(' ');
                    skill c_skill = new skill();
                    c_skill.position_id = j - i;
                    c_skill.skill_name = skillline[0];
                    c_skill.skill_level = int.Parse(skillline[1]);
                    my_contributer.conributer_skills.Add(c_skill);
                    // n_of_contributer+=j;
                    // i+=j;
                }
                i += my_contributer.n_contributer_skills;
                n_of_contributer = n_of_contributer + my_contributer.n_contributer_skills;
                lines_till_proj = i + 1;

                my_contributers.Add(my_contributer);
            }
            // // get p
            for (int i = lines_till_proj; i < lines_till_proj + n_of_project; i++)
            {

                var currentline = MyInputFile[i].Split(' ');
                project my_project = new project();
                my_project.proj_id = i - lines_till_proj;
                my_project.project_name = currentline[0];
                my_project.duration_to_complete_day = int.Parse(currentline[1]);
                my_project.score = int.Parse(currentline[2]);
                my_project.best_before_day = int.Parse(currentline[3]);
                if (last_day < my_project.best_before_day)
                    last_day = my_project.best_before_day;
                my_project.n_skills = int.Parse(currentline[4]);
                int skill_position = 0;
                for (int j = i + 1; j <= i + my_project.n_skills; j++)
                {
                    var skillline = MyInputFile[j].Split(' ');
                    skill c_skill = new skill();
                    c_skill.position_id = skill_position;
                    c_skill.skill_name = skillline[0];
                    c_skill.skill_level = int.Parse(skillline[1]);
                    my_project.project_skills.Add(c_skill);
                    skill_position += 1;
                }
                i += my_project.n_skills;
                n_of_project = n_of_project + my_project.n_skills;

                my_projects.Add(my_project);
            }
            my_projects = my_projects.OrderBy(p => p.best_before_day).ThenBy(p => p.duration_to_complete_day).ToList();
            Console.WriteLine("n of con " + my_contributers.Count().ToString() + " - " +
            "n of projects " + my_projects.Count().ToString());

        }
    }
    class contributer
    {
        public string contributer_name { get; set; }
        public int n_contributer_skills { get; set; }
        public bool is_available { get; set; } = true;
        public bool can_upgrade { get; set; } = false;
        public skill skill_to_upgrade { get; set; }
        // public int c_level { get; set; }
        public List<skill> conributer_skills { get; set; } = new List<skill>();
    }
    class skill
    {
        public skill()
        {
        }

        public skill(int position_id, string skill_name, int skill_level, bool is_taken)
        {
            this.position_id = position_id;
            this.skill_name = skill_name;
            this.skill_level = skill_level;
            this.is_taken = is_taken;
        }

        public int position_id { get; set; }
        public string skill_name { get; set; }
        public int skill_level { get; set; }
        public bool is_taken { get; set; } = false;
    }
    class project
    {
        public int proj_id { get; set; }
        public string project_name { get; set; }
        public int duration_to_complete_day { get; set; }
        public int best_before_day { get; set; }
        public int score { get; set; }
        public int n_skills { get; set; }
        public List<skill> project_skills { get; set; } = new List<skill>();
        public bool executed { get; set; } = false;
    }

    class project_best_by
    {
        public project_best_by() { }
        public project_best_by(List<int> proj_id_list, int best_by_date)
        {
            this.best_by_date = best_by_date;
            this.proj_id_list = proj_id_list;
        }
        public List<int> proj_id_list { get; set; }
        public int best_by_date { get; set; }
    }
    class project_duration
    {
        public project_duration() { }
        public project_duration(List<int> proj_id_list, int best_by_date, int duration_day)
        {
            this.duration_day = duration_day;
            this.proj_id_list = proj_id_list;
        }

        public List<int> proj_id_list { get; set; }
        public int duration_day { get; set; }
    }

    class project_timeline
    {
        public project_timeline() { }
        public project_timeline(List<int> proj_id_list, int best_by_date, int day_needed_to_finish)
        {
            this.best_by_date = best_by_date;
            this.day_needed_to_finish = day_needed_to_finish;
            this.sort_criteria = int.Parse(day_needed_to_finish.ToString() + best_by_date.ToString());
            this.proj_id_list = proj_id_list;
        }

        public List<int> proj_id_list { get; set; }
        public int sort_criteria { get; set; }
        public int best_by_date { get; set; }
        public int day_needed_to_finish { get; set; }
    }

    class output
    {
        public int n_execeuted_projects { get; set; }
        public List<op_2> project_info { get; set; } = new List<op_2>();
    }

    class op_2
    {
        public string project_name { get; set; }
        public List<string> contributer_name_list { get; set; } = new List<string>();
    }
}
