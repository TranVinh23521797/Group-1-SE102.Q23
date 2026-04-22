using UnityEngine;
using TMPro;
using IrishFarmSim;

public class CashDisplay : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI cashText;

    void OnEnable()
    {
        // 1. Kiểm tra xem cashText đã được gán chưa
        if (cashText == null)
        {
            Debug.LogWarning("cashText chưa được gán trong Inspector!");
            return;
        }

        // 2. Lấy instance và kiểm tra an toàn
        // (Xóa () nếu Instance của bạn là Property)
        var controller = GameController.Instance();

        if (controller != null && controller.player != null)
        {
            // Cập nhật UI (Đã bỏ ký hiệu $ đi cho hợp lý với VND)
            cashText.text = $"Cash: {controller.player.cash:F2} VND";
        }
        else
        {
            // Có thể gán text mặc định nếu dữ liệu chưa sẵn sàng
            cashText.text = "Cash: 0 VND";
        }
    }
}
