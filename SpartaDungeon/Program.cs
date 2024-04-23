namespace SpartaDungeon
{
    public enum SHOPPING
    { 
        Success, //구매 성공! → 정수형으로 변환하면 0
        Insufficient, //gold가 부족! → 정수형으로 변환하면 1
        SoldOut, //이미 판매됨! → 정수형으로 변환하면 2
        WrongInput //잘못된 입력값! → 정수형으로 변환하면 3
    }
    class Character
    {
        int command;
        //레벨, 공격력, 방어력, 체력, 골드
        int level, attack, defense, hp, gold;
        //이름, 전직
        string name, job;

        public Character(string name, int level, string job,
            int attack, int defense, int hp, int gold)
        {
            this.name = name;
            this.level = level;
            this.job = job;
            this.attack = attack;
            this.defense = defense;
            this.hp = hp;
            this.gold = gold;
        }
        public int Gold { get { return gold; }  }
        public void setGold(int minus)
        {
            this.gold -= minus;
        }
        public void showCharacterInfo()
        {
            Console.Clear();

            Program.showColorRed("< 상태 보기 >\n");
            Console.WriteLine("캐릭터의 정보가 표시됩니다.\n");

            Console.WriteLine("Lv. " + level.ToString("00"));
            Console.WriteLine($"{name} ( {job} )");
            Console.WriteLine("공격력 : " + attack);
            Console.WriteLine("방어력 : " + defense);
            Console.WriteLine("체력 : " + hp);
            Console.WriteLine("Gold : {0} G", gold);

            Console.WriteLine();

            command = Program.getCommand();
            if (command == 0)
                return;
        }

    }
    class Inventory
    {
        int command;
        //소지 중인 아이템을 저장할 List
        List<Item> items;
        public Inventory(List<Item> items)
        {
            this.items = items;
        }
        void showInventoryList(bool isSetting)
        {
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.\n");

            Program.showColorYellow("[ 아이템 목록 ]");

            if (items.Count == 0) Console.WriteLine("암것도 없다"); //이게 출력이 되네요

            int idx = 1;
            foreach (Item item in items)
            {
                if (item.getIsSoldOut()) 
                {
                    //장착 페이지로 넘어가면 목록 앞에 숫자 출력
                    if (isSetting)
                    {
                        item.showList((idx++).ToString());
                    }
                    else
                        item.showList("-");
                }
                    
            }
            
        }
        public void showInventoryInfo()
        {
            while (true)
            {
                Console.Clear();

                Program.showColorRed("< 인벤토리 >\n");

                showInventoryList(false);

                Console.WriteLine("\n\n1. 장착 관리\n0. 나가기\n");
                command = Program.getCommand();

                if (command == 1)
                {
                    itemEquip();
                }
                //입력값이 0이라면 showInventoryInfo()를 호출한 곳으로 돌아감(이전 화면으로 돌아감)
                else if (command == 0)
                    return;
            }
        }
        public void itemEquip()
        {
            Console.Clear();
            while(true)
            {
                Console.Clear();
                Program.showColorRed("< 인벤토리 - 장착 관리 >\n");
                showInventoryList(true);

                Console.WriteLine();
                Console.WriteLine();
                command = Program.getCommand();
                if (command == 0) return;
            }
        }
    }
    class Shop
    {
        int command;
        Character character;
        List<Item> items;
        public Shop(Character character, List<Item> items)
        {
            this.character = character;
            this.items = items;
        }
        void showShoppingList(bool isBuying)
        {
            Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.\n");

            Program.showColorYellow("[보유 골드]\n");
            Console.WriteLine($"{character.Gold} G");

            Program.showColorYellow("\n[아이템 목록]");

            foreach (Item item in items)
            { 
                //구매 페이지로 넘어가면 목록 앞에 숫자 출력
                if(isBuying)
                {
                    int index = items.IndexOf(item);
                    item.showList( (index + 1).ToString());
                }
                else
                    item.showList("-");
            }
        }
        //상점의 초기 메뉴
        public void showShopInfo()
        {
            while (true)
            {
                Console.Clear();

                Program.showColorRed("< 상점 >\n");
                showShoppingList(false);
            
                Console.WriteLine("\n\n1. 아이템 구매\n0. 나가기\n");

            
                command = Program.getCommand();
                //입력값이 1이라면 아이템 구매 메소드 호출
                if (command == 1)
                {
                    buyItem();
                }
                //입력값이 0이라면 showShopInfo()를 호출한 곳으로 돌아감(이전 화면으로 돌아감)
                else if (command == 0)
                    return;
            }
        }
        //아이템 구매를 위한 기능
        public void buyItem()
        {
            //입력에 대한 상태를 저장하는 변수
            //state는 0부터 3까지의 값(enum SHOPPING)을 가지며,
            //각 값은 아이템을 구매할 때 발생할 수 있는 상황에 대한 결과를 나타낸다.
            int state = -1; 
            Console.Clear();

            while(true)
            {
                Console.Clear();

                Program.showColorRed("< 상점 - 아이템 구매 >\n");
                showShoppingList(true);

                Console.WriteLine();
                if (state >= 0)
                {
                    ShowCurrnetState(ref state);
                }

                Console.WriteLine();
                Console.WriteLine();
                command = Program.getCommand();

                if (command < 0 || command > items.Count)
                {
                    state = (int)SHOPPING.WrongInput;
                    continue;
                }
                else if (command == 0) return;

                foreach (Item item in items)
                {
                    //현재 입력한 숫자(인덱스과 비교하므로 1 감소)에 해당하는 아이템이 
                    if ((command - 1) == items.IndexOf(item))
                    {
                        //판매되지 않은 상태라면, 
                        if (!item.getIsSoldOut())
                        {
                            //소지하고 있는 금액이 아이템의 가격보다 많을 때, 
                            if (character.Gold >= item.getPrice())
                            {
                                //해당 아이템을 판매 완료 상태로 변경
                                item.setSoldOut();

                                //현재 소지금에서 아이템의 가격을 차감
                                character.setGold(item.getPrice());
                                state = (int)SHOPPING.Success;

                                break;
                            }
                            //소지 금액이 가격보다 더 적은 경우, 
                            else
                            {
                                state = (int)SHOPPING.Insufficient; break;
                            }
                        }
                        //판매 된 상태라면, 
                        else
                        {
                            state = (int)SHOPPING.SoldOut; break;
                        }
                    }
                }
            }
        }
        public void ShowCurrnetState(ref int state)
        {
            Console.WriteLine();
            switch (state)
            {
                case (int)SHOPPING.Success:
                    Program.showColorYellow("구매를 완료했습니다.");
                    break;
                case (int)SHOPPING.Insufficient:
                    Program.showColorRed("!! Gold가 부족합니다."); break;
                case (int)SHOPPING.SoldOut:
                    Program.showColorYellow("!! 이미 구매한 아이템입니다."); break;
                case (int)SHOPPING.WrongInput:
                    Program.showColorRed("잘못된 입력입니다.");
                    break;
                default:
                    break;
            }
            state = -1;
        }
    }
    class Item
    {
        string name, description;
        int plusAttack, plusDefense, price;

        //구매 하였는지에 대한 여부를 저장하는 변수, 장비를 하였는지에 대한 여부를 저장하는 변수
        bool isSoldOut = false, isEquipped = false;

        public int getPrice() { return price; }
        public bool getIsSoldOut() { return isSoldOut; }
        public void setSoldOut() { isSoldOut = true; }

        public Item(string name, string description, int plusAttack,
            int plusDefense, int price)
        {
            this.name = name; this.description = description;
            this.plusAttack = plusAttack; this.plusDefense = plusDefense;
            this.price = price;
        }
        public void showList(string s)
        {
            Console.WriteLine();

            //장착중이라면, [E]를 표시
            if (isEquipped)
                Program.showColorYellow("[E] ");
            Program.showColorRed(s + " ");
            Console.Write($"{name,-12}\t|");
            if (plusAttack != 0)
                Console.Write($"공격력 +{plusAttack,-9}\t|");
            if (plusDefense != 0)
                Console.Write($"방어력 +{plusDefense,-9}\t|");
            Console.Write($"{description,-35}\t|");

            if (!isSoldOut)
                Console.Write($"{price,-5}G");
            else Program.showColorYellow("구매 완료");

        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Character player
                = new Character("홍길동", 1, "전사", 10, 5, 100, 1500);

            List<Item> items = new List<Item>();
            setItemList(items);

            Inventory inventory = new Inventory(items);
            Shop shop = new Shop(player,items);

            while (true)
            {
                Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
                Console.WriteLine("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.\n");

                showMenu();

                Console.Write("원하시는 행동을 입력해주세요(0 → 프로그램 종료).\n>> ");
                int command = int.Parse(Console.ReadLine());
                if (command == 0)
                {
                    Console.WriteLine("던전 게임을 종료합니다."); break;
                }

                switch (command)
                {
                    case 1:
                        player.showCharacterInfo();
                        break;
                    case 2:
                        inventory.showInventoryInfo();
                        break;
                    case 3:
                        shop.showShopInfo();
                        break;
                    default:
                        Console.WriteLine("잘못된 입력입니다.");
                        break;
                }

                Console.Clear();
            }
        }

        static void showMenu()
        {
            Console.WriteLine("1. 상태 보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 상점");
            Console.WriteLine();
        }
        static void setItemList(List<Item> list)
        {
            list.Add(new Item("수련자 갑옷", "수련에 도움을 주는 갑옷입니다.", 0, 5, 1000));
            list.Add(new Item("무쇠 갑옷", "무쇠로 만들어져 튼튼한 갑옷입니다.", 0, 9, 200));
            list.Add(new Item("스파르타의 갑옷", "스파르타의 전사들이 사용했다는 전설의 갑옷입니다."
                , 0, 15, 3500));
            list.Add(new Item("낡은 검", "쉽게 볼 수 있는 낡은 검입니다.", 2, 0, 600));
            list.Add(new Item("청동 도끼", "어디선가 사용됐던거 같은 도끼입니다.", 5, 0, 1500));
            list.Add(new Item("스파르타의 창", "스파르타의 전사들이 사용했다는 전설의 창입니다.",
                7, 0, 2000));
        }
        public static void showColorRed(string str)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write(str);
            Console.ResetColor();
        }
        public static void showColorYellow(string str)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write(str);
            Console.ResetColor();
        }
        public static int getCommand()
        {

            Console.Write("원하시는 행동을 입력해주세요(0 → 이전으로 돌아가기).\n>> ");
            int command = int.Parse(Console.ReadLine());
            return command;
        }
    }
}
