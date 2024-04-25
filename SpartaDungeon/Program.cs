

using Newtonsoft.Json;
using System;
using System.IO;
using System.Numerics;
namespace SpartaDungeon
{
    public enum SHOPPING
    {
        Success, //구매 성공! → 정수형으로 변환하면 0
        Insufficient, //gold가 부족! → 정수형으로 변환하면 1
        SoldOut, //이미 판매됨! → 정수형으로 변환하면 2
    }

    //잘못된 입력을 판별하기 위한 구조체
    public enum CommandValid
    {
        InValid = -1,
    }
    public enum ItemType
    {
        Armor,
        Weapon,
    }
    public class Character
    {
        int command;
        //레벨, 공격력, 방어력, 체력, 골드
        int level, attack, defense, hp, gold;
        //이름, 전직
        string name, job;

        int clearCNT = 0;

        //방어구, 무기에 대한 장착 정보를 저장할 변수
        Item? itemWeapon, itemArmor;

        int itemAttack = 0, itemDefense = 0;
        public Character() { }
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

        public int ItemAttack { get { return itemAttack; } set { itemAttack = value; } }
        public int ItemDefense { get { return itemDefense; } set { itemDefense = value; } }
        public int Level { get { return level; } set { level += value; } }
        public int HP { get { if (hp < 0) hp = 0; return hp; } set { hp = value; } }
        public int Attack { get { return attack; } set { attack += value; } }
        public int Defense{ get { return defense; } set { defense += value; } }
        public int Gold { get { return gold; } set { gold = value; } }
        public string Name { get { return name; }set { name = value; } }
        public string Job { get { return job; } set { job = value; } }
        public int ClearCNT { get { return clearCNT; } set { clearCNT = value; } }
        public Item Weapon { get { return itemWeapon; } set { itemWeapon = value; } }
        public Item Armor { get { return itemArmor; } set { itemArmor = value; } }
        public void setGold(int minus)
        {
            this.gold -= minus;
            if(this.gold <=0)
                gold = 0;
        }

        //장착한 아이템에 대하여 능력치를 적용해주는 함수
        public void setItemState(int attack, int defense)
        {
            this.attack += attack; this.defense += defense;
            itemAttack += attack; itemDefense += defense;
        }
        public void showCharacterInfo()
        {
            int a = Attack;
            if (a < 0) a = 0;
            int d = Defense;
            if (d < 0) d = 0;

            while (true)
            {
                Console.Clear();

                Program.showColorRed("< 상태 보기 >\n");
                Console.WriteLine("캐릭터의 정보가 표시됩니다.\n");

                Console.WriteLine("Lv. " + level.ToString("00"));
                Console.WriteLine($"{name} ( {job} )");
                Console.Write("공격력 : " + a);

                if (itemAttack != 0) //아이템으로 인해 공격력에 증감이 있다면, 
                {
                    Console.Write(" (");
                    if (itemAttack < 0)
                        Program.showColorRed(itemAttack.ToString());
                    else Program.showColorYellow("+" + itemAttack);
                    Console.WriteLine(")");
                }
                else Console.WriteLine();

                Console.Write("방어력 : " + d);

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

                if (command == (int)CommandValid.InValid)
                {
                    Program.showColorRed("잘못된 입력입니다.\n"); ;
                }

                command = Program.CheckCommandVaild(0, 0);

                if (command == 0)
                    return;
            }
        }

    }
    public class Inventory
    {
        int command;
        //List<Item> items;
        //소지 중인 아이템을 저장할 List
        private List<Item> invenItems = new List<Item>();
        public List<Item>  InvenItems { get { return invenItems;  } set { invenItems = value; } }
        Character character;
        public Inventory() { }
        public Inventory(Character character)
        {
            //this.items = items;
            this.character = character;
        }

        public void showInventoryList(bool isSetting)
        {

            Program.showColorYellow("[아이템 목록]");

            int idx = 1;

            foreach (Item item in InvenItems)
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
                Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.\n");

                showInventoryList(false);

                Console.WriteLine("\n\n1. 장착 관리\n0. 나가기\n");

                if (command == (int)CommandValid.InValid)
                {
                    Program.showColorRed("잘못된 입력입니다.\n"); ;
                }

                command = Program.CheckCommandVaild(0, 1);

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
                Console.WriteLine("보유 중인 아이템을 관리할 수 있습니다.\n");

                showInventoryList(true);

                Console.WriteLine();
                Console.WriteLine();
                if (command == (int)CommandValid.InValid)
                {
                    Program.showColorRed("잘못된 입력입니다.\n"); ;
                }
                command = Program.CheckCommandVaild(0, InvenItems.Count);

                if (command == 0) return;

                foreach (Item item in InvenItems)
                {
                    if ((command - 1) == InvenItems.IndexOf(item))
                    {
                        item.setItemEquip();

                        //현재의 item.getItemEquip()의 값이 true라면 장착 X -> 장착 O 상태로 간 것.
                        if (item.IsEquipped)
                        {

                            //장착하려는 아이템의 유형이 무기일 때, 
                            if (item.Type == ItemType.Weapon)
                            {
                                //현재 캐릭터가 무기를 장착한 상태라면, 
                                if (character.Weapon !=null)
                                {
                                    //이전에 장착한 아이템에 대한 정보를 불러와 장착 상태를 해제
                                    Item preItem = character.Weapon;
                                    preItem.setItemEquip();

                                    int attack = (-1 * preItem.PlusAttack);
                                    int denfense = (-1 * preItem.PlusDefense);
                                    character.setItemState(attack, denfense);
                                }

                                //현재 아이템을 장착중인 아이템으로 등록
                                character.Weapon = item;

                            }
                            //장착하려는 아이템의 유형이 방어구일 때,
                            if(item.Type == ItemType.Armor)
                            {
                                
                                if (character.Armor != null)
                                {
                                    Item preItem = character.Armor;
                                    preItem.setItemEquip();

                                    int attack = (-1 * preItem.PlusAttack);
                                    int denfense = (-1 * preItem.PlusDefense);
                                    character.setItemState(attack, denfense);
                                }
                                character.Armor = item;
                            }

                            //아이템의 능력치만큼 캐릭터의 공격력이나 방어력에 가산 연산
                            character.setItemState(item.PlusAttack, item.PlusDefense);
                        }
                        //현재의 item.getItemEquip()의 값이 false라면 장착 O -> 장착 X 상태로 간 것.
                        else
                        {
                            //장착을 해제할 때 아이템의 타입에 따라 무기/방어구 정보를 null로 바꾸어줌
                            if (item.Type == ItemType.Weapon)
                                character.Weapon = null;
                            if (item.Type == ItemType.Armor)
                                character.Armor = null;

                            //원래의 스텟에 -1을 곱한 결과로 가산 연산을 진행하므로 결과적으로 값이 줄어듬.
                            int attack = (-1 * item.PlusAttack);
                            int denfense = (-1 * item.PlusDefense);
                            character.setItemState(attack , denfense);
                        }
                    }
                }
            }
        }
    }
    public class Shop
    {
        int command;
        Character character;
        List<Item> items;
        Inventory inventory;

        public Shop(Character character, List<Item> items, Inventory inventory)
        {
            this.character = character;
            this.items = items;
            this.inventory = inventory;
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

                Console.WriteLine("\n\n1. 아이템 구매\n2. 아이템 판매\n0. 나가기\n");

                if (command == (int)CommandValid.InValid)
                {
                    Program.showColorRed("잘못된 입력입니다.\n"); ;
                }

                command = Program.CheckCommandVaild(0, 2);

                //입력값이 1이라면 아이템 구매 메소드 호출
                if (command == 1)
                {
                    buyItem();
                }
                else if(command == 2)
                {
                    sellItem();
                }
                //입력값이 0이라면 showShopInfo()를 호출한 곳으로 돌아감(이전 화면으로 돌아감)
                else if (command == 0)
                    return;
            }
        }
        //아이템 구매를 위한 메서드
        void buyItem()
        {
            //입력에 대한 상태를 저장하는 변수
            //state는 0부터 3까지의 값(enum SHOPPING)을 가지며,
            //각 값은 아이템을 구매할 때 발생할 수 있는 상황에 대한 결과를 나타낸다.
            int state = -1;
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
                if (command == (int)CommandValid.InValid)
                {
                    Program.showColorRed("잘못된 입력입니다.\n"); ;
                }

                command = Program.CheckCommandVaild(0, items.Count);

                if (command == 0) return;

                foreach (Item item in items)
                {
                    //현재 입력한 숫자(인덱스과 비교하므로 1 감소)에 해당하는 아이템이 
                    if ((command - 1) == items.IndexOf(item))
                    {
                        //판매되지 않은 상태라면, 
                        if (!item.IsSoldOut)
                        {
                            //소지하고 있는 금액이 아이템의 가격보다 많을 때, 
                            if (character.Gold >= item.Price)
                            {
                                //해당 아이템을 판매 완료 상태로 변경
                                item.setSoldOut();

                                //현재 소지금에서 아이템의 가격을 차감
                                character.setGold(item.Price);
                                state = (int)SHOPPING.Success;

                                inventory.InvenItems.Add(item);
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
        //아이템 판매를 위한 메서드
        void sellItem()
        {
            bool sellComplete = false;
            
            while (true)
            {
                Console.Clear();
                Program.showColorRed("< 상점 - 아이템 판매 >\n");
                Console.WriteLine("필요한 아이템을 얻을 수 있는 상점입니다.\n");

                Program.showColorYellow("[보유 골드]\n");
                Console.WriteLine($"{character.Gold} G\n");

                inventory.showInventoryList(true);

                Console.WriteLine();
                Console.WriteLine();
                if (command == (int)CommandValid.InValid)
                {
                    Program.showColorRed("잘못된 입력입니다.\n");
                }

                if(sellComplete)
                {
                    Program.showColorGreen("판매 완료!\n");
                    sellComplete = false;
                }

                command = Program.CheckCommandVaild(0, inventory.InvenItems.Count);
                if (command == 0) return;


                foreach (Item item in inventory.InvenItems)
                {
                    if ((command - 1) == inventory.InvenItems.IndexOf(item))
                    {
                        //선택한 아이템이 구매가 된 상태라면, 
                        if (item.IsSoldOut)
                        {
                            item.setSoldOut();

                            int refund = (int)Math.Round(item.Price * 0.85);
                            character.setGold(-1 * refund);

                            if (item.IsEquipped) 
                            {
                                item.setItemEquip();

                                int attack = (-1 * item.PlusAttack);
                                int denfense = (-1 * item.PlusDefense);
                                character.setItemState(attack, denfense);
                            }

                            inventory.InvenItems.Remove(item);
                            sellComplete = true;
                            break;
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
                default:
                    break;
            }
            state = -1;
        }
    }
    public class Dungeon
    {
        Character character;

        int command;
        int[] setDefenseHard = [5, 11, 17];
        string[] DungeonName = ["쉬운 던전", "일반 던전", "어려운 던전"];
        int[] reward = [1000, 1700, 2500];

        public Dungeon(Character character)
        {
            this.character = character;
        }

        public void showDungeonInfo()
        {
            while(true)
            {
                Console.Clear();

                Program.showColorRed("< 던전 입장 >\n");
                Console.WriteLine("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.\n");

                for(int i=0;i<3;i++)
                {
                    Program.showColorYellow((i+1).ToString());
                    Console.Write($". {DungeonName[i],10}\t| ");
                    Console.Write("방어력 ");
                    Program.showColorYellow(setDefenseHard[i].ToString());
                    Console.WriteLine(" 이상 권장");
                }

                if (command == (int)CommandValid.InValid)
                {
                    Program.showColorRed("잘못된 입력입니다.\n");
                }

                Console.WriteLine();
                command = Program.CheckCommandVaild(0, 3);
                if (command == 0) return;

                CheckDungeonClear(command);
            }
        }
        void CheckDungeonClear(int idx)
        {
            //권장 방어력보다 높을 경우 무조건 클리어이므로,
            //isClear의 초기값을 true로 설정
            bool isClear = true;

            //권장 방어력보다 낮을 경우,
            if (character.Defense < setDefenseHard[idx-1])
            {
                //num에는 0부터 9까지의 숫자가 랜덤으로 저장.
                int rnd = new Random().Next(0, 10);

                //만일 랜덤으로 나온 rnd의 결과가 0,1,2,3 中 하나라면(40%),
                //던전 실패 isClear = false
                isClear = rnd > 4;
            }

            Console.Clear();

            if (isClear) 
            {
                Program.showColorRed("< 던전 클리어 >\n");
                Console.WriteLine("축하합니다!!");
                Console.WriteLine($"{DungeonName[idx-1]}을 클리어 하였습니다.\n");

                Program.showColorYellow("[탐험 결과]\n");

                int minusHp = setDefenseHard[idx-1] - character.Defense;
                int rnd = new Random().Next((20 + minusHp), (35 + minusHp));

                int hp = character.HP - rnd;

                //공격력 ~ 공격력*2% 만큼의 추가 보상
                rnd = new Random().Next(character.Attack , 2 * character.Attack);

                //클리어 기본 보상 + 클리어 기본 보상의 (공격력 ~ 공격력x2) % 만큼의 추가 보상
                int Gold = reward[idx - 1] + reward[idx - 1] * rnd / 100;
                
                Console.Write($"체력 {character.HP} ");
                Program.showColorYellow("→ ");
                character.HP = hp;
                Console.WriteLine($"{character.HP}\n");

                Console.Write($"Gold {character.Gold} ");
                character.setGold(-1 * Gold);
                Program.showColorYellow("→ ");
                Console.WriteLine($"{character.Gold}\n");

                character.ClearCNT = character.ClearCNT + 1;

                if (character.Level == character.ClearCNT)
                {
                    character.ClearCNT = 0;
                    Program.showColorGreen("!!레벨업을 축하드립니다!!\n");

                    Program.showColorYellow("[Level UP!]\n");
                    Console.WriteLine($"{character.Level}LV → {character.Level + 1}LV\n");
                    character.Level = 1;

                    //기본 공격력은 0.5, 방어력은 1 증가
                    Program.showColorYellow("[공격력] ");
                    Console.Write($"\n{character.Attack} → ");
                    int num = (int)(character.Attack * 0.5); //현재 공격력의 50%
                    character.Attack = num;
                    Console.WriteLine($"{character.Attack}\n");

                    Program.showColorYellow("[방어력] ");
                    Console.Write($"\n{character.Defense} → ");
                    num = (int)(character.Defense * 0.3); //현재 방어력의 30%
                    character.Defense = num;
                    Console.WriteLine($"{character.Defense}\n");
                }
            }
            else
            {
                int hp = character.HP / 2;

                Program.showColorRed("< 던전 공략 실패 >\n");
                Console.WriteLine($"{DungeonName[idx-1]}을 클리어하지 못했습니다.\n");

                Program.showColorYellow("[탐험 결과]\n");
                Console.Write($"체력 {character.HP} ");
                Program.showColorYellow("→ ");
                Console.WriteLine($"{hp}\n");

                character.HP = hp;
            }
            command = Program.CheckCommandVaild(0, 0);
            if (command == 0) return;
        }
    }
    public class Item
    {
        string name, description;

        //아이템의 공격력, 방어력, 가격
        int plusAttack, plusDefense, price;

        //아이템이 방어구인지 무기인지 구별하기 위한 열거형 변수
        ItemType type;

        //+,- 부호를 저장할 변수 (공격력용, 방어력용)
        string markA, markD;

        //구매 하였는지에 대한 여부를 저장하는 변수, 장비를 하였는지에 대한 여부를 저장하는 변수
        bool isSoldOut = false, isEquipped = false;

        public int Price { get { return price; } set { price = value; } }
        public bool IsSoldOut { get { return isSoldOut; } set { isSoldOut = value; } }
        public string Name { get { return name; } set { name = value; } }
        public string Description { get { return description; } set { description = value; } }
        public bool IsEquipped { get { return isEquipped; } set { isEquipped = value; } }
        public int PlusAttack { get { return plusAttack; } set { PlusAttack = value; } }
        public int PlusDefense { get { return plusDefense; } set { plusDefense = value; } }
        public ItemType Type { get { return type; } set { Type = value; } }

        public void setSoldOut() { isSoldOut = !isSoldOut; }

        public void setItemEquip() { isEquipped = !isEquipped; }

        public Item(string name, string description, int plusAttack,
            int plusDefense, int price, ItemType type)
        {
            this.name = name; this.description = description;
            this.plusAttack = plusAttack; this.plusDefense = plusDefense;
            this.price = price;
            this.type = type;
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
    class Break
    {
        int command;
        Character character;
        public Break(Character character) {  this.character = character; }
        public void showBreakInfo()
        {
            while (true)
            {
                Console.Clear();
                
                Program.showColorRed("< 휴식하기 >\n");
                Program.showColorYellow("500");
                Console.Write("G를 내면 체력을 회복할 수 있습니다. ");
                Console.Write("(보유 골드 :");
                Program.showColorGreen($"{character.Gold} ");
                Console.WriteLine("G)\n");

                Program.showColorYellow("[현재 체력] - ");
                Console.WriteLine($"(100/{character.HP})\n");

                Program.showColorYellow("1. ");
                Console.WriteLine("휴식하기");
                Program.showColorYellow("0. ");
                Console.WriteLine("나가기");

                if (command == (int)CommandValid.InValid)
                {
                    Program.showColorRed("잘못된 입력입니다.\n");
                }

                command = Program.CheckCommandVaild(0, 1);
                if (command == 0) return;
                if (command == 1) takeBreak();
            }
        }
        void takeBreak()
        {
            Console.Clear();
            Program.showColorRed("< 휴식하기 >\n");
            Console.WriteLine("휴식을 마쳤습니다! 힘이 솟아나는 기분입니다.\n");

            Program.showColorYellow("[탐험 결과]\n");
            Console.Write($"체력 {character.HP} ");
            Program.showColorYellow("→ ");
            character.HP = 100;
            Console.WriteLine($"{character.HP}\n");

            if (command == (int)CommandValid.InValid)
            {
                Program.showColorRed("잘못된 입력입니다.\n");
            }
            command = Program.CheckCommandVaild(0, 0);
            if (command == 0) return;
        }
    }
    internal class Program
    {
        static void Main(string[] args)
        {
            Character player = new Character();
            List<Item> items = new List<Item>();
            Inventory inventory = new Inventory();

            LoadGameData(ref player, ref items, ref inventory);

            Dungeon dungeon = new Dungeon(player);

            Shop shop = new Shop(player, items, inventory);
            Break takeBreak = new Break(player);

            int? command = null;

            while (true)
            {
                Console.Clear();

                Console.WriteLine("스파르타 마을에 오신 여러분 환영합니다.");
                Console.WriteLine("이곳에서 던전으로 들어가기 전 활동을 할 수 있습니다.\n");

                showMenu();

                if (command == (int)CommandValid.InValid)
                {
                    showColorRed("잘못된 입력입니다.\n");
                }

                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.Write("0번을 누르면 지금까지의 상태를 저장하고 종료합니다.\n");
                Console.ResetColor();

                command = CheckCommandVaild(0, 5);
                
                if (command == 0)
                {
                    Console.WriteLine("\n던전 게임을 종료합니다."); break;
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
                    case 4:
                        dungeon.showDungeonInfo(); break;
                    case 5:
                        takeBreak.showBreakInfo(); break;
                    default:
                        break;
                }
            }

            //================== 데이터 저장하는 부분(플레이어, 아이템, 인벤토리 정보) ==================
           
            string path = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string filePath = Path.Combine(path, "saveFile.json");
            string itemFilePath = Path.Combine(path, "saveItemFile.json");
            string ivenFilePath = Path.Combine(path, "saveInvenFile.json");

            var playerJson = JsonConvert.SerializeObject(player,Formatting.Indented);
            var ItemJson = JsonConvert.SerializeObject(items, Formatting.Indented);
            var InvenJson = JsonConvert.SerializeObject(inventory, Formatting.Indented);

            File.WriteAllText(filePath, playerJson, System.Text.Encoding.UTF8);
            File.WriteAllText(itemFilePath, ItemJson, System.Text.Encoding.UTF8);
            File.WriteAllText(ivenFilePath, InvenJson, System.Text.Encoding.UTF8);

            Console.WriteLine("저장이 완료되었습니다! ");
        }
        
        //데이터 로드
        static void LoadGameData(ref Character player, ref List<Item> items,ref Inventory inventory)
        {
            string playerFileName = "saveFile.json";
            string itemFileName = "saveItemFile.json";
            string invenFileName =  "saveInvenFile.json";

            string userDocumentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            // 플레이어 데이터 로드
            string playerFilePath = Path.Combine(userDocumentsFolder, playerFileName);
            if (File.Exists(playerFilePath))
            {
                string playerJson = File.ReadAllText(playerFilePath, System.Text.Encoding.UTF8);
                player = JsonConvert.DeserializeObject<Character>(playerJson);
            }
            else
            {
                player = new Character("홍길동", 1, "전사", 10, 5, 100, 1500);
            }

            // 아이템 데이터 로드
            string itemFilePath = Path.Combine(userDocumentsFolder, itemFileName);
            if (File.Exists(itemFilePath))
            {
                string itemJson = File.ReadAllText(itemFilePath, System.Text.Encoding.UTF8);
                items = JsonConvert.DeserializeObject<List<Item>>(itemJson);
            }
            else
            {
                setItemList(items);
            }

            string invenFilePath = Path.Combine(userDocumentsFolder, invenFileName);
            if(File.Exists(invenFilePath))
            {
                string invenJson = File.ReadAllText(invenFilePath, System.Text.Encoding.UTF8);
                inventory = JsonConvert.DeserializeObject<Inventory>(invenJson);
            }
            else
            {
                inventory = new Inventory(player);
                
            }
        }
        static void showMenu()
        {
            Console.WriteLine("1. 상태 보기");
            Console.WriteLine("2. 인벤토리");
            Console.WriteLine("3. 상점");
            Console.WriteLine("4. 던전 입장");
            Console.WriteLine("5. 휴식하기");
            Console.WriteLine();
        }
        static void setItemList(List<Item> list)
        {
            list.Add(new Item("수련자 갑옷", "수련에 도움을 주는 갑옷입니다.", 0, 5, 1000, ItemType.Armor));
            list.Add(new Item("무쇠 갑옷", "무쇠로 만들어져 튼튼한 갑옷입니다.", 0, 9, 200, ItemType.Armor));
            list.Add(new Item("스파르타의 갑옷", "스파르타의 전사들이 사용했다는 전설의 갑옷입니다.", 0, 15, 3500, ItemType.Armor));
            list.Add(new Item("저주받은 갑옷", "그 어떠한 공격도 막아낼 수 있으나...", -30, 40, 8000, ItemType.Armor));

            list.Add(new Item("낡은 검", "쉽게 볼 수 있는 낡은 검입니다.", 2, 0, 600,ItemType.Weapon));
            list.Add(new Item("청동 도끼", "어디선가 사용됐던거 같은 도끼입니다.", 5, 0, 1500, ItemType.Weapon));
            list.Add(new Item("스파르타의 창", "스파르타의 전사들이 사용했다는 전설의 창입니다.", 7, 0, 2000,ItemType.Weapon));
            list.Add(new Item("타노스의 건틀렛", "큰 힘에는 큰 책임이 따릅니다.", 20, -10, 5000, ItemType.Weapon));
        }

        //문자열의 색을 달리 출력하게 해주는 메서드
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

        //함수의 매개변수로 커맨드의 입력 범위를 받음. 
        //ex) 0 - 나가기, 1 - 구매하기, 2 - 판매하기 → start는 0, end는 2.
        public static int CheckCommandVaild(int start, int end)
        {
            Console.Write("원하시는 행동을 입력해주세요(0 → 이전으로 돌아가기).\n>> ");
            string command = Console.ReadLine();
            int input;

            //입력한 커맨드가 int로 변환이 불가능하거나, 명령으로 등록되지 않은 숫자를 입력하였을 경우, 
            if (!int.TryParse(command, out input) || input < start || input > end)
            {
                return -1;
            }

            return input;
        }
    }
}
