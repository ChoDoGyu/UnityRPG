using UnityRPG.Item;

namespace UnityRPG.DeveloperTools
{
    public sealed class GiveItemCommand : IConsoleCommand
    {
        private readonly PlayerInventory inventory;
        private readonly ItemDatabase itemDatabase;

        public string Name => "give";
        public string Description => "플레이어에게 아이템을 지급합니다.";
        public string Usage => "give <itemId> <amount>";

        public GiveItemCommand(PlayerInventory inventory, ItemDatabase itemDatabase)
        {
            this.inventory = inventory;
            this.itemDatabase = itemDatabase;
        }

        public ConsoleCommandResult Execute(string[] args)
        {
            if (args.Length != 2)
                return ConsoleCommandResult.Fail($"사용법: {Usage}");

            if (!itemDatabase.TryGetItem(args[0], out ItemDefinition item))
                return ConsoleCommandResult.Fail($"아이템을 찾을 수 없습니다: {args[0]}");

            if (!int.TryParse(args[1], out int amount) || amount <= 0)
                return ConsoleCommandResult.Fail("수량은 1 이상의 정수여야 합니다.");

            int addedAmount = inventory.AddItem(item, amount);

            if (addedAmount <= 0)
                return ConsoleCommandResult.Fail("인벤토리에 공간이 없습니다.");

            if (addedAmount < amount)
                return ConsoleCommandResult.Succeed($"인벤토리 공간 부족으로 {item.ItemId} {addedAmount}/{amount}개 지급");

            return ConsoleCommandResult.Succeed($"{item.ItemId} {addedAmount}개 지급");
        }
    }
}