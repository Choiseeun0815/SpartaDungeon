
using System;

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

        int itemAttack = 0, itemDefense = 0;

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
        public int Gold { get { return gold; } }
        public void setGold(int minus)
        {
            this.gold -= minus;
        }

        //장착한 아이템에 대하여 능력치를 적용해주는 함수
        public void setItemState(int attack, int defense)
        {
            this.attack += attack; this.defense += defense;
            itemAttack += attack; itemDefense += defense;
        }
        public void showCharacterInfo()
        {
            Console.Clear();

            Program.showColorRed("< 상태 보기 >\n");
            Console.WriteLine("캐릭터의 정보가 표시됩니다.\n");

            Console.WriteLine("Lv. " + level.ToString("00"));
            Console.WriteLine($"{name} ( {job} )");
            Console.Write("공격력 : " + attack);

            if (itemAttack != 0) //아이템으로 인해 공격력에 증감이 있다면, 
            {
                Console.Write(" (");  
                if(itemAttack < 0)
                    Program.showColorRed(itemAttack.ToString());
                else Program.showColorYellow("+" + itemAttack);
                Console.WriteLine(")");
            }
            else Console.WriteLine();

            Console.Write("방어력 : " + defense);

            if (itemDefense != 0) //아이템으로 인해 방어력력에 증감이 있다면, 
            {
                Console.Write(" (");
                if (itemDefense < 0)
                    Program.showColorRed(itemDefense.ToString());
                else Program.showColorYellow("+" + itemDefense);
                Console.WriteLine(")");
            }
            else Console.WriteLine();

            Console.WriteLine("체력 : " + hp);
            Console.WriteLine("Gold : {0}G", gold);

            Console.WriteLine();

            command = Program.getCommand();
            if (command == 0)
                return;
        }

    }
    class Inventory
    {
        int command;
        bool isFrist = true;
        //소지 중인 아이템을 저장할 List
        List<Item> items;
        List<Item> invenItems = new List<Item>();

        Character character;
        public Inventory(List<Item> items, Character character)
        {
            this.items = items;
            this.character = character;
        }

        void showInventoryList(bool isSetting)
        {
            Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.\n");

            Program.showColorYellow("[ 아이템 목록 ]");

            int idx = 1;
            //딱 한 번만 실행!
            if (isFrist)
            {
                foreach (Item item in items)
                {
                    if (item.getIsSoldOut())
                    {
                        invenItems.Add(item); //새로운 리스트 Item 리스트 변수에 구매한 아이템을 할당
                    }
                }
                isFrist = false;
            }

            //if (newItems.Count == 0) Console.WriteLine("암것도 없다"); //이게 출력이 되네요
            //Console.WriteLine("암것도 없다 " + newItems.Count); //이게 출력이 되네요

            foreach (Item item in invenItems)
            {
                //장착 관리 페이지라면 숫자를 표시
                if (isSetting)
                {
                    item.showList((idx++).ToString(), true);
                }
                else
                    item.showList("-", true);
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
                    equipItem();
                }
                //입력값이 0이라면 showInventoryInfo()를 호출한 곳으로 돌아감(이전 화면으로 돌아감)
                else if (command == 0)
                    return;
            }
        }
        public void equipItem()
        {
            Console.Clear();
            while (true)
            {
                Console.Clear();
                Program.showColorRed("< 인벤토리 - 장착 관리 >\n");

                showInventoryList(true);

                Console.WriteLine();
                Console.WriteLine();
                command = Program.getCommand();

                //일치하는 아이템을 선택하지 않았다면,
                if (command < 0 || command > invenItems.Count)
                {
                    Program.showColorRed("잘못된 입력입니다.");
                    continue;
                }
                else if (command == 0) return;

                foreach (Item item in invenItems)
                {
                    //입력한 번호에 해당하는 아이템의 장착 상태를 true라면 false로, false라면 true로 변경시켜준다.
                    if ((command - 1) == invenItems.IndexOf(item))
                    {
                        item.setItemEquip();

                        //현재의 item.getItemEquip()의 값이 true라면 장착 X -> 장착 O 상태로 간 것.
                        //아이템의 능력치만큼 캐릭터의 공격력이나 방어력에 가산 연산
                        if (item.getItemEquip())
                        {
                            character.setItemState(item.getPlusAttack(), item.getPlusDefense());
                        }

                        //현재의 item.getItemEquip()의 값이 false라면 장착 O -> 장착 X 상태로 간 것.
                        //원래의 스텟에 -1을 곱한 결과로 가산 연산을 진행하므로 결과적으로 값이 줄어듬.
                        else
                        {
                            int attack = (-1 * item.getPlusAttack());
                            int denfense = (-1 * item.getPlusDefense());
                            character.setItemState(attack, denfense);
                        }
                    }
                }
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
                if (isBuying)
                {
                    int index = items.IndexOf(item);
                    item.showList((index + 1).ToString(), false);
                }
                else
                    item.showList("-", false);
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

            while (true)
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
                    Program.showColorGreen("구매를 완료했습니다.");
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
        //아이템의 공격력, 방어력, 가격
        int plusAttack, plusDefense, price;

        //아이템의 능력치가 양수인지 음수인지 판단하기 위한 변수
        bool isAttackPositive = true, isDefensePositive = true;

        //+,- 부호를 저장할 변수 (공격력용, 방어력용)
        string markA, markD;

        //구매 하였는지에 대한 여부를 저장하는 변수, 장비를 하였는지에 대한 여부를 저장하는 변수
        bool isSoldOut = false, isEquipped = false;

        //< 프로퍼티로 변경하기 !!!!!!!!!!!!!!!!!!!!! >
        public int getPrice() { return price; }

        public bool getIsSoldOut() { return isSoldOut; }
        public void setSoldOut() { isSoldOut = true; }

        public void setItemEquip() { isEquipped = !isEquipped; }
        public bool getItemEquip() { return isEquipped; }

        public int getPlusAttack() { return plusAttack; }
        public int getPlusDefense() { return plusDefense; }

        public Item(string name, string description, int plusAttack,
            int plusDefense, int price)
        {
            this.name = name; this.description = description;
            this.plusAttack = plusAttack; this.plusDefense = plusDefense;
            this.price = price;
        }
        public void showList(string s, bool isFromInven)
        {
            markA = (plusAttack > 0) ? "+" : "";
            markD = (plusDefense > 0) ? "+" : "";

            Console.WriteLine();

            Program.showColorRed(s + " ");

            //인벤토리에서 장착 여부를 확인 가능. 상점은 X
            if (isFromInven)
            {
                //장착중이라면, [E]를 표시
                if (isEquipped)
                    Program.showColorYellow("[E] ");
            }

            Console.Write($"{name,-12}\t|");

            //아이템이 공격력과 방어력 모두에게 영향을 미치는 경우, 
            if (plusAttack != 0 && plusDefense != 0)
            {
                Console.Write($"공격력 {markA}{plusAttack}, ");
                Console.Write($"방어력 {markD}{plusDefense,-4}|");
            }
            else 
            {
                if (plusAttack != 0)
                    Console.Write($"공격력 {markA}{plusAttack,-9}\t|");
                if (plusDefense != 0)
                    Console.Write($"방어력 {markD}{plusDefense,-9}\t|");
            }
            
            Console.Write($"{description,-35}\t");

            //인벤토리에서 아이템의 정보를 표시해줄 때에는 구매 정보나 가격 표시 X
            if (!isFromInven)
            {
                if (!isSoldOut)
                    Console.Write($"|{price,-5}G");
                else Program.showColorYellow("|구매 완료");
            }

        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            List<Item> items = new List<Item>();
            setItemList(items);


            Character player = new Character("홍길동", 1, "전사", 10, 20, 100, 11500);

            Shop shop = new Shop(player, items);
            Inventory inventory = new Inventory(items, player);

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
            list.Add(new Item("스파르타의 갑옷", "스파르타의 전사들이 사용했다는 전설의 갑옷입니다.", 0, 15, 3500));

            list.Add(new Item("낡은 검", "쉽게 볼 수 있는 낡은 검입니다.", 2, 0, 600));
            list.Add(new Item("청동 도끼", "어디선가 사용됐던거 같은 도끼입니다.", 5, 0, 1500));
            list.Add(new Item("스파르타의 창", "스파르타의 전사들이 사용했다는 전설의 창입니다.", 7, 0, 2000));
            list.Add(new Item("타노스의 건틀렛", "큰 힘에는 큰 책임이 따릅니다.", 20, -10, 5000));
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
        public static void showColorGreen(string str)
        {
            Console.ForegroundColor = ConsoleColor.Green;
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
