using Assets.Scripts.Client;
using Assets.Scripts.Space;
using System.Collections;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Zenject;

public class DoorLike : MonoBehaviour
{
    [Header("Showing likes")]
    [SerializeField] private DoorLikeView _doorLikeView; //hall visual likes
    [Space, Header("Recieving likes")]
    [SerializeField] private LikeInput _likeInput; //space like buttons
    [SerializeField] private XRSimpleInteractable _likeButton;
    [SerializeField] private XRSimpleInteractable _dislikeButton;
    [SerializeField] private HandButton _likePhysicsButton;
    [SerializeField] private HandButton _dislikePhysicsButton;
   
    private ISpaceManager _spaceManager;
    private ISpaceService _spaceService;
    private IClientData _clientData;
    private ILikeService _likeService;
    private LocalSpace _space; //the space which this door represents in Hall or in Space
    private bool _wasLikedByMe = false;
    private bool _wasDislikedByMe = false;
    private Coroutine _doubleClickPreventCoroutine;

    [Inject]
    private void Construct(ISpaceManager spaceManager, IClientData clientData, ILikeService likeService, ISpaceService spaceService)
    {
        _spaceManager = spaceManager;
        _clientData = clientData;
        _likeService = likeService;
        _spaceService = spaceService;
    }

    private void Start()
    {
#if !UNITY_SERVER
        
        if (_spaceManager != null && _spaceManager.CurrentTransitionTarget != null && _spaceManager.CurrentTransitionTarget.Id != null)
        {
            _spaceService.ReloadSpaces();

            var targetSpace = _spaceManager.CurrentTransitionTarget;
            _space = _spaceService.Get(targetSpace.SpaceType, targetSpace.Id.ToString());
        }
            
        CheckIfLikableInSpace();
#endif
    }

    private void OnEnable()
    {
        _likeButton.hoverEntered.AddListener(OnHoverEnteredLike);
        _likeButton.hoverExited.AddListener(OnHoverExitedLike);
        _likeButton.activated.AddListener(OnActivateLike);
        _likePhysicsButton.OnPressPhysicsButton += Like;

        _dislikeButton.activated.AddListener(OnActivateDislike);
        _dislikeButton.hoverEntered.AddListener(OnHoverEnteredDislike);
        _dislikeButton.hoverExited.AddListener(OnHoverExitedDislike);
        _dislikePhysicsButton.OnPressPhysicsButton += Dislike;
    }

    /// <summary>
    /// Using in Hall to show likes count
    /// </summary>
    /// <param name="space"></param>
    public void SetSpaceFromHall(LocalSpace space)
    {
        _space = space;

        if (_space.CanLike == null || !_space.CanLike.Value || _doorLikeView == null)
        {
            _doorLikeView.gameObject.SetActive(false);
            return;
        }

        int likesCount = _space.LikesNumber != null ? _space.LikesNumber.Value : 0;
        int dislikesCount = _space.DislikesNumber != null ? _space.DislikesNumber.Value : 0;

        if (likesCount == 0 && dislikesCount == 0)
        {
            _doorLikeView.gameObject.SetActive(false);
            return;
        }

        _doorLikeView.gameObject.SetActive(true);

        _doorLikeView.SetLikesCount(likesCount);

        if (likesCount + dislikesCount == 0)
        {
            _doorLikeView.SetStarsCount(-1); //-1 meens stars are not shown
        }
        else
        {
            int rating = likesCount * 100 / (likesCount + dislikesCount);
            if (rating > 80) _doorLikeView.SetStarsCount(5);
            else if (rating > 60) _doorLikeView.SetStarsCount(4);
            else if (rating > 40) _doorLikeView.SetStarsCount(3);
            else if (rating > 20) _doorLikeView.SetStarsCount(2);
            else if (rating >= 1) _doorLikeView.SetStarsCount(1);
            else _doorLikeView.SetStarsCount(0);
        }
    }
     
    /// <summary>
    /// Using in Space to show like buttons
    /// </summary>
    /// <returns></returns>
    private void CheckIfLikableInSpace()
    {
        if (_space != null && 
            _space.CanLike != null && 
            _space.CanLike.Value && 
            _space.SpaceType != SpaceType.HallScene)
        {
            _likeInput.gameObject.SetActive(true);
            if (!_space.Like.HasValue)
            {
                _likeInput.SetLikePressed(false);
                _likeInput.SetDisLikePressed(false);
                _wasLikedByMe = false;
                _wasDislikedByMe = false;
            }
            else if (_space.Like == true)
            {
                _likeInput.SetLikePressed(true);
                _likeInput.SetDisLikePressed(false);
                _wasLikedByMe = true;
                _wasDislikedByMe = false;
            }
            else if (_space.Like == false)
            {
                _likeInput.SetLikePressed(false);
                _likeInput.SetDisLikePressed(true);
                _wasDislikedByMe = true;
                _wasLikedByMe = false;
            }
        }        
    }

    private void OnHoverExitedDislike(HoverExitEventArgs arg0)
    {
        
    }

    private void OnHoverEnteredDislike(HoverEnterEventArgs arg0)
    {
        
    }

    private void OnActivateDislike(ActivateEventArgs arg0)
    {
        Dislike();
    }

    private void Dislike()
    {      
        if (!_wasLikedByMe && !_wasDislikedByMe)
        {
            OnDislikePressed(false);
            _likeInput.SetDisLikePressed(true);
            _wasDislikedByMe = true;
            return;
        }
        
        if (_wasDislikedByMe)
        {
            OnDislikePressed(true);
            _likeInput.SetDisLikePressed(false);
            _wasDislikedByMe = false;
            _wasLikedByMe = false;
            return;
        }
        
        if (_wasLikedByMe)
        {
            OnDislikePressed(false);
            _likeInput.SetDisLikePressed(true);
            _likeInput.SetLikePressed(false);
            _wasDislikedByMe = true;
            _wasLikedByMe = false;
        }
    }

    private void OnActivateLike(ActivateEventArgs arg0)
    {
        Like();
    }

    private void Like()
    {      
        if (!_wasLikedByMe && !_wasDislikedByMe)
        {
            OnLikePressed(false);
            _likeInput.SetLikePressed(true);
            _wasLikedByMe = true;            
            return;
        }

        if (_wasLikedByMe)
        {
            OnLikePressed(true);
            _likeInput.SetLikePressed(false);
            _wasDislikedByMe = false;
            _wasLikedByMe = false;            
            return;
        }

        if (_wasDislikedByMe)
        {
            OnLikePressed(false);
            _likeInput.SetDisLikePressed(false);
            _likeInput.SetLikePressed(true);
            _wasDislikedByMe = false;
            _wasLikedByMe = true;
        }
    }

    private void OnHoverExitedLike(HoverExitEventArgs arg0)
    {
        
    }

    private void OnHoverEnteredLike(HoverEnterEventArgs arg0)
    {
       
    }

    private void OnDisable()
    {
        _likeButton.hoverEntered.RemoveListener(OnHoverEnteredLike);
        _likeButton.hoverExited.RemoveListener(OnHoverExitedLike);
        _likeButton.activated.RemoveListener(OnActivateLike);
        _dislikeButton.activated.RemoveListener(OnActivateDislike);
        _dislikeButton.hoverEntered.RemoveListener(OnHoverEnteredDislike);
        _dislikeButton.hoverExited.RemoveListener(OnHoverExitedDislike);

        _likePhysicsButton.OnPressPhysicsButton -= Like;
        _dislikePhysicsButton.OnPressPhysicsButton -= Dislike;
    }
    

    private void OnLikePressed(bool isTwicePress)
    {
        if (_doubleClickPreventCoroutine != null) StopCoroutine(_doubleClickPreventCoroutine);
        _doubleClickPreventCoroutine = StartCoroutine(DoubleClickProtector());

        if (isTwicePress) _likeService.Like(_space.Id, null, _clientData);
        else _likeService.Like(_space.Id, true, _clientData); // _clientData.UserId.Value
    }

    private void OnDislikePressed(bool isTwicePress)
    {
        if (_doubleClickPreventCoroutine != null) StopCoroutine(_doubleClickPreventCoroutine);
        _doubleClickPreventCoroutine = StartCoroutine(DoubleClickProtector());

        if (isTwicePress) _likeService.Like(_space.Id, null, _clientData);
        else _likeService.Like(_space.Id, false, _clientData);
    }    

    private IEnumerator DoubleClickProtector()
    {        
        _likePhysicsButton.OnPressPhysicsButton -= Like;
        _dislikePhysicsButton.OnPressPhysicsButton -= Dislike;

        yield return new WaitForSeconds(0.3f);
        
        _likePhysicsButton.OnPressPhysicsButton += Like;
        _dislikePhysicsButton.OnPressPhysicsButton += Dislike;
    }
}
