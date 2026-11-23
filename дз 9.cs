/*
Задача 1.1
1. Игрок и ловушка реализуют IDamageable, поэтому их можно добавить в один список. ProcessDamage вызывает TakeDamage без проверки типа, а каждый объект реагирует на урон по-своему.
2. У ловушки есть всё, что требует интерфейс: HP и метод TakeDamage. Если false означает, что объект не пережил удар, то вернуть его нормально. Странно другое, ловушка уже сломана, а у неё всё ещё 10 HP, и изменить это значение нельзя.
3. Зелье в этот список добавить нельзя: оно реализует IHealable, а список принимает только IDamageable. При обычном добавлении будет ошибка компиляции, а при приведении через object будет ошибка во время выполнения. Чтобы использовать зелье, нужно вызвать Heal.
4. Если добавить IsDead в IDamageable, его нужно реализовать и у игрока, и у ловушки. У игрока можно проверить, осталось ли у него здоровье, у ловушки — хранить признак того, что она сломана. Зелье менять не надо, у него другой интерфейс.

Задача 1.2
Абстрактный класс + Интерфейс +
У интерфейса есть только статические поля, у абстрактного класса — ещё и поля объекта.

Абстрактный класс + Интерфейс -
Конструктор экземпляра есть только у класса; у интерфейса возможен статический.

Абстрактный класс - Интерфейс +
Базовый класс может быть один, а интерфейсов — несколько.

Абстрактный класс + Интерфейс +
Оба могут содержать готовый код метода, интерфейс — начиная с C# 8.

Абстрактный класс + Интерфейс +
Оба могут объявлять свойства, реализацию которых напишет наследник или реализующий класс.

Абстрактный класс + Интерфейс +
Оба подходят для переменной со ссылкой на объект конкретного класса.

Абстрактный класс + Интерфейс -
Класс может наследовать другой класс, интерфейс — только интерфейсы.

Абстрактный класс + Интерфейс +
Оба допускают приватные методы с телом, интерфейс — начиная с C# 8.


Дополнительный вопрос

Наследование:
Player -> Character
NPC    -> Character
Enemy  -> Character

Реализация интерфейсов:
Player -> IInteractable
Player -> IHealable
NPC    -> IInteractable

Абстрактный класс объединяет общие данные и логику.
В Character это имя, здоровье и получение урона. Интерфейсы задают
отдельные возможности: IInteractable — диалог, IHealable — лечение.
*/

using System;
using System.Collections.Generic;

internal static class Program
{
    private static void Main()
    {
        Console.WriteLine("2.1. Бой рыцаря и мага");
        ICombatant knight = new Knight("Артур");
        ICombatant mage = new Mage("Мерлин");

        while (!knight.IsDead && !mage.IsDead)
        {
            knight.Attack(mage);
            if (!mage.IsDead)
                mage.Attack(knight);
        }

        Console.WriteLine(mage.IsDead ? $"Победил рыцарь {knight.Name}!" : $"Победил маг {mage.Name}!");

        Console.WriteLine("\n2.2. Использование предметов");
        var player = new Player("Алекс", 100, 20);
        var items = new List<IUsableItem> { new HealthPotion(), new Sword(), new AncientScroll() };
        foreach (IUsableItem item in items)
            item.Use(player);

        string state = player.HP > 0 ? "жив" : "мёртв";
        Console.WriteLine($"{player.Name} — HP: {player.HP}, урон: {player.Damage}, состояние: {state}");

        Console.WriteLine("\n2.3. Виды урона");
        var unit = new Unit("Страж", 200, 40);
        int baseDamage = 60;

        Console.WriteLine("Физический урон");
        new DamageSystem(new PhysicalDamageCalculator()).ApplyDamage(unit, baseDamage);
        Console.WriteLine("Магический урон");
        new DamageSystem(new MagicDamageCalculator()).ApplyDamage(unit, baseDamage);
        Console.WriteLine("Огненный урон");
        new DamageSystem(new FireDamageCalculator()).ApplyDamage(unit, baseDamage);
    }
}

public interface ICombatant
{
    string Name { get; }
    int HP { get; set; }
    bool IsDead { get; }
    int Damage { get; }
    void Attack(ICombatant target);
    void TakeDamage(int amount);
}

public class Knight : ICombatant
{
    public string Name { get; }
    public int HP { get; set; } = 150;
    public bool IsDead => HP <= 0;
    public int Damage { get; } = 25;

    public Knight(string name)
    {
        Name = name;
    }

    public void Attack(ICombatant target)
    {
        if (IsDead)
        {
            Console.WriteLine($"Рыцарь {Name} мёртв и не может атаковать");
            return;
        }

        if (target.IsDead)
        {
            Console.WriteLine($"Рыцарь {Name} не может атаковать мёртвую цель");
            return;
        }

        Console.WriteLine($"Рыцарь {Name} атакует!");
        target.TakeDamage(Damage);
    }

    public void TakeDamage(int amount)
    {
        HP = Math.Max(0, HP - amount);
        Console.WriteLine($"Рыцарь {Name} получил {amount} урона. Осталось HP: {HP}");
    }
}

public class Mage : ICombatant
{
    public string Name { get; }
    public int HP { get; set; } = 80;
    public bool IsDead => HP <= 0;
    public int Damage { get; } = 40;

    public Mage(string name)
    {
        Name = name;
    }

    public void Attack(ICombatant target)
    {
        if (target.IsDead)
        {
            Console.WriteLine($"Маг {Name} не может атаковать мёртвую цель");
            return;
        }

        if (IsDead)
        {
            Console.WriteLine($"Маг {Name} мёртв и не может атаковать");
            return;
        }

        Console.WriteLine($"Маг {Name} атакует двойным заклинанием!");
        target.TakeDamage(Damage);
        target.TakeDamage(Damage);
    }

    public void TakeDamage(int amount)
    {
        HP = Math.Max(0, HP - amount);
        Console.WriteLine($"Маг {Name} получил {amount} урона. Осталось HP: {HP}");
    }
}

public enum Rarity
{
    Common,
    Rare,
    Legendary
}

public interface IUsableItem
{
    string Name { get; }
    string Description { get; }
    Rarity Rarity { get; }
    void Use(Player player);
}

public class Player
{
    public int HP;
    public int Damage;
    public string Name;

    public Player(string name, int hp, int damage)
    {
        Name = name;
        HP = hp;
        Damage = damage;
    }

    public void Heal(int amount)
    {
        HP += amount;
    }

    public void IncreaseDamage(int amount)
    {
        Damage += amount;
    }
}

public class HealthPotion : IUsableItem
{
    public string Name => "Зелье здоровья";
    public string Description => "Восстанавливает 50 HP";
    public Rarity Rarity => Rarity.Common;

    public void Use(Player player)
    {
        player.Heal(50);
        Console.WriteLine("Вы выпили зелье и восстановили 50 HP");
    }
}

public class Sword : IUsableItem
{
    public string Name => "Меч";
    public string Description => "Увеличивает урон на 10";
    public Rarity Rarity => Rarity.Rare;

    public void Use(Player player)
    {
        player.IncreaseDamage(10);
        Console.WriteLine("Вы взяли меч, и ваш урон вырос на 10");
    }
}

public class AncientScroll : IUsableItem
{
    public string Name => "Древний свиток";
    public string Description => "Снимает 20 HP и увеличивает урон на 30";
    public Rarity Rarity => Rarity.Legendary;

    public void Use(Player player)
    {
        player.HP = Math.Max(0, player.HP - 20);
        player.IncreaseDamage(30);
        Console.WriteLine("Свиток проклял вас на 20 урона и увеличил ваш урон на 30");
    }
}

public interface IDamageCalculator
{
    int CalculateDamage(int baseDamage, int targetArmor);
}

public class PhysicalDamageCalculator : IDamageCalculator
{
    public int CalculateDamage(int baseDamage, int targetArmor)
    {
        return Math.Max(1, baseDamage - targetArmor / 2);
    }
}

public class MagicDamageCalculator : IDamageCalculator
{
    public int CalculateDamage(int baseDamage, int targetArmor)
    {
        if (targetArmor == 0)
            return Math.Max(1, baseDamage * 2);

        return Math.Max(1, baseDamage - targetArmor / 4);
    }
}

public class FireDamageCalculator : IDamageCalculator
{
    public int CalculateDamage(int baseDamage, int targetArmor)
    {
        int damage = Math.Max(1, baseDamage - targetArmor / 10);
        if (baseDamage > 50)
            damage += 10;

        return damage;
    }
}

public class Unit
{
    public string Name { get; }
    public int HP { get; set; }
    public int Armor { get; }

    public Unit(string name, int hp, int armor)
    {
        Name = name;
        HP = hp;
        Armor = armor;
    }
}

public class DamageSystem
{
    private readonly IDamageCalculator _calculator;

    public DamageSystem(IDamageCalculator calculator)
    {
        _calculator = calculator;
    }

    public void ApplyDamage(Unit target, int baseDamage)
    {
        int damage = _calculator.CalculateDamage(baseDamage, target.Armor);
        target.HP = Math.Max(0, target.HP - damage);
        Console.WriteLine($"Юнит {target.Name} получил {damage} урона. Осталось HP: {target.HP}");
    }
}

// Для ледяного урона я бы сделал класс IceDamageCalculator с интерфейсом.
// IDamageCalculator и написал в нём свою формулу. Потом передал бы его в конструктор DamageSystem. В самом DamageSystem ничего бы не менял.