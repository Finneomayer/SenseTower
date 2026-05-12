using Assets.Scripts.API;
using Assets.Scripts.Hall;
using Assets.Scripts.Space;

public class MyPlaceAccessTypeController
{
    private MyPlaceAccessTypeView _view;
    private LocalSpace _model;
    private IApiService _apiService;

    public MyPlaceAccessTypeController(IApiService apiService)
    {
        _apiService = apiService;
    }

    public void Init(MyPlaceAccessTypeView view, LocalSpace model)
    {
        _view = view;
        _model = model;

        _view.AccessTypeChanged += SetAccessType;
    }

    public void RefreshView()
    {
        _view.SetAccessType(_model.PublicAccessType);
    }

    private async void SetAccessType(SpaceAccessType accessType)
    {
        _view.SetInteractable(false);
        if (accessType == SpaceAccessType.Paid)
        {
            await _apiService.UpdatePlaceAccessType(_model.Id, accessType, _view.Tax);
        }
        else
        {
            await _apiService.UpdatePlaceAccessType(_model.Id, accessType);
        }

        _model.PublicAccessType = accessType;
        _view.SetAccessType(accessType);

        _view.SetInteractable(true);
    }
}
